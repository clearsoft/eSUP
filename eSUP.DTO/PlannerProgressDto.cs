namespace eSUP.DTO;

public class PlannerProgressDto
{
    public string? Title { get; set; } // Planner Title
    public List<HeaderDto> HeadingItems { get; set; } = [];
    public List<StudentProgressDto> StudentProgresses { get; set; } = [];
}
