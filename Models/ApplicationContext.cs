using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Operation> Operations { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Cabinet> Cabinets { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
             : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated(); 
        }
    }
}
