namespace WebApplication1.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<Operation>? Operations { get; set; } = new();
    }
}
