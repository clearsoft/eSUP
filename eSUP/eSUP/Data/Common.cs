namespace eSUP.Data
{
    public class Common
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } = "System";
        public int Sequence { get; set; } = 0;
        public string? Title { get; set; } = string.Empty;
    }
}
