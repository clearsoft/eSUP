namespace eSUP.DTO;

public class StudentProgressDto : CommonDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName ?? ""} {LastName ?? ""}";
    public string? EMail { get; set; }
    public List<int> PartsCompleteCount { get; set; } = [];
    public List<ExerciseDto> Exercises { get; set; } = [];
}



