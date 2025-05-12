namespace eSUP.Model;

public class Part : Common
{
    public bool IsLevelBelow { get; set; }
    public bool IsLevelAt { get; set; }
    public bool IsLevelAbove { get; set; }
    public PartCompletionState State { get; set; } = PartCompletionState.NotStarted;
    public List<ApplicationUser> Users { get; set; } = [];
}
