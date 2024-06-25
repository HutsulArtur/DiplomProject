using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Models;
using System.Web;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        ApplicationContext db;
        List<Doctor> Docs;
        List<Appointment> Apps,Apps2;
        List<Operation> Actions;
        List<Cabinet> Cabs;
        DateOnly d;
        public ManagerController(ApplicationContext context)
        {
            db = context;
            Docs = db.Doctors.ToList();
            Cabs = db.Cabinets.ToList();
            Actions = db.Operations.Include(u=>u.Doctors).ToList();
            d = DateOnly.FromDateTime(DateTime.Now);
            Apps = db.Appointments.OrderBy(u=>u.Date).
                ThenBy(u=>u.Time).Where(u=> u.Date >= d).
                Include(u => u.Patient).
                Include(m => m.Operation).
                Include(m => m.Doctor).
                ToList();
        }

        public IActionResult OldAppointments()
        {
            d = DateOnly.FromDateTime(DateTime.Now);
            Apps2 = db.Appointments.OrderByDescending(u => u.Date).
            ThenByDescending(u => u.Time).Where(u => u.Date <= d).
            Include(u => u.Patient).
            Include(m => m.Operation).
            Include(m => m.Doctor).
            ToList();
            return View(Apps2);
        }


        public IActionResult Index()
        {
            /*
            ViewBag.name = "";
            ViewBag.password = "";
            if (HttpContext.Request.Cookies.ContainsKey("name"))
            {
                ViewBag.name = HttpContext.Request.Cookies["name"];
                ViewBag.password = HttpContext.Request.Cookies["password"];
            }
            */
            return View();
        }
        [HttpGet]
        public IActionResult Cabinets()
        {
            return View(Cabs);
        }
        [HttpPost]
        public IActionResult Cabinets(string name)
        {
            
            if (name != null) {
                Cabinet cab = new() { Name = name };
                db.Cabinets.Add(cab);
                db.SaveChanges();
                Cabs = db.Cabinets.ToList();
                return View(Cabs);
            } else return View("Error");

        }
        [HttpGet]
        public IActionResult Doctors()
        {
            return View(Docs);
        }
        [HttpPost]
        public IActionResult Doctors(string name, string phone, string description)
        {
            if (name != null && phone != null && description != null) 
            {
                Doctor doc = new() { Name = name, Phone = phone, Description = description };
                db.Doctors.Add(doc);
                db.SaveChanges();
                Docs = db.Doctors.ToList();
                return View(Docs);
            } else return View("Error");

        }
        [HttpGet]
        public IActionResult Operations()
        {
            ViewBag.Docs = Docs;
            return View(Actions);
        }
        [HttpPost]
        public IActionResult Operations(string name, int price, int duration, string[] doctor) //
        {
            if (name != null && doctor.Length > 0)
            {
                Models.Operation act = new() { Name = name, Price = price, Duration = duration };
                act.Doctors = new();
                foreach (string strq in doctor)
                {
                    Doctor? df = db.Doctors?.FirstOrDefault(u => u.Name == strq);
                    act.Doctors.Add(df);
                }
                db.Operations.Add(act);
                db.SaveChanges();
                Docs = db.Doctors.ToList();
                Actions = db.Operations.Include(u => u.Doctors).ToList();
                ViewBag.Docs = Docs;
                return View(Actions);
            } else return View("Error");

        }


        public IActionResult Appointments()
        {
            return View(Apps);
        }

        public IActionResult Autorization(string name, string password)
        {
                return View("Index");
        }

        public IActionResult EditAppointment(string id)
        {
            //Apps = db.Appointments.Include(u => u.Patient).ToList();
            Appointment? app = db.Appointments.FirstOrDefault(u => u.Id == Int32.Parse(id));

            ViewBag.Docs = db.Doctors.ToList();
            ViewBag.Cabs = db.Cabinets.ToList();
            ViewBag.Opers = db.Operations.ToList();
            return View(app);
        }
        //public IActionResult Message() => View();
        public IActionResult SaveEditedApp(string patient, string doctor, DateOnly date, TimeOnly time, string operation, string cabinet, string id)
        {
            Appointment? app = db.Appointments.FirstOrDefault(u => u.Id == Int32.Parse(id));
            app.Patient= db.Patients.FirstOrDefault(u => u.Name == patient);
            app.Doctor = db.Doctors.FirstOrDefault(u => u.Name == doctor);
            app.Cabinet = db.Cabinets.FirstOrDefault(u => u.Name == cabinet);
            app.Operation = db.Operations.FirstOrDefault(u => u.Name == operation);
            app.Date = date;
            app.Time = time;
            db.Appointments.Update(app);
            db.SaveChanges();

            return View("Message");
        }

        public IActionResult DeleteAppointment(string id)
        {
            Appointment? app = db.Appointments.Find(Int32.Parse(id));
            db.Appointments.Remove(app);
            db.SaveChanges();
            return View("Delete");
        }
    }
}
