namespace eSUP.Data
{
    public class Planner : Common
    {
        public string? Subject { get; set; }
        public string? Unit { get; set; }
        public string? Topic { get; set; }
        public List<Exercise> Exercises { get; set; } = [];
        public List<ApplicationUser> Users { get; set; } = [];
    }
}
