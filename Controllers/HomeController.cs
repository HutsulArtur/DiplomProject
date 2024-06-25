//using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        ApplicationContext db;
        List<Doctor> Doctors = new List<Doctor>();
        List<Operation> Operations = new();
        public HomeController(ApplicationContext context)
        {
            db = context;
            Doctors = db.Doctors.Include(u => u.Operations).ToList();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Schedule()
        {
            return View(Doctors);
        }

        public IActionResult Write()
        {
            return View(Doctors);
        }

        public IActionResult WriteDoctor(string name)
        {
            if (name==null) return View("Write", Doctors);
            Doctor ? doc = db.Doctors.FirstOrDefault(u=>u.Name==name);
            Operations = db.Operations.Include(u => u.Doctors).Where(u => u.Doctors.Contains(doc)).ToList();
            ViewData["Doctor"] = doc.Name;
            
            return View(Operations);
        }
        public IActionResult WriteAppointment(string name, string doctor, DateOnly date, TimeOnly time, string action, string phone)
        {
            if (name == null || doctor == null || action == null || phone == null)
                return View("Message");
            Doctor? doc = db.Doctors.FirstOrDefault(u => u.Name == doctor);
            Models.Operation? act = db.Operations.Where(u=>u.Name==action).Include(u =>u.Doctors).Where(u=>u.Doctors.Contains(doc)).FirstOrDefault();
            Patient patient = new() { Name = name, Phone = phone };
            Appointment appointment = new() { Patient = patient, Operation = act, Date=date, Time=time, Doctor=doc};
            db.Appointments.Add(appointment);
            db.SaveChanges();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}