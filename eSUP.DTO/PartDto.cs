namespace eSUP.DTO;

public class PartDto : CommonDto
{
    public bool IsEnabled { get; set; }
    public bool IsLevelBelow { get; set; }
    public bool IsLevelAt { get; set; }
    public bool IsLevelAbove { get; set; }
    public bool IsCompleted { get; set; }
}
