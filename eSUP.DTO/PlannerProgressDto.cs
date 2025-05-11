namespace eSUP.DTO;

public class PlannerProgressDto : CommonDto
{
    public List<HeaderDto> HeadingItems { get; set; } = [];
    public List<StudentProgressDto> StudentProgresses { get; set; } = [];
}
