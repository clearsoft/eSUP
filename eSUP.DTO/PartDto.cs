namespace eSUP.DTO;

public class PartDto : CommonDto
{
    public bool IsEnabled { get; set; } = true;
    public bool IsLevelBelow { get; set; } = false;
    public bool IsLevelAt { get; set; } = false;
    public bool IsLevelAbove { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
}
