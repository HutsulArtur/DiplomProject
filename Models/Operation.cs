namespace WebApplication1.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int Duration { get; set; }
        public List<Doctor>? Doctors { get; set; } = new();
    }

}
