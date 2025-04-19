namespace eSUP.DTO;

public class PartChangeDto
{
    public Guid PartId { get; set; }
    public bool IsLevelBelow { get; set; }
    public bool IsLevelAt { get; set; }
    public bool IsLevelAbove { get; set; }
}
