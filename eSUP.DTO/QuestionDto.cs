namespace eSUP.DTO;

public class QuestionDto : CommonDto
{
    public List<PartDto> Parts { get; set; } = [];
    public bool State { get; set; } = false; // used locally to set/clear all parts
}
