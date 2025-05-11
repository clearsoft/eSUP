namespace eSUP.DTO;

public class StudentProgressDto : CommonDto
{
    public string? Name { get; set; }
    public List<int> PartsCompleteCount { get; set; } = [];
    public List<ExerciseDto> Exercises { get; set; } = [];
}



