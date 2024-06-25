namespace WebApplication1.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public Operation? Operation { get; set; } = null!;
        public Doctor? Doctor { get; set; } = null!;
        public Cabinet? Cabinet { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
    }
}
