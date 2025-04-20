namespace eSUP.DTO;

public class StudentProgressDto
{
    public string? Name { get; set; }
    public List<bool> IsPartCompleteList { get; set; } = [];
    public List<int> PartsCompleteCount { get; set; } = [];
}
