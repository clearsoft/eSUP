namespace eSUP.DTO;

public class UserAssignmentUpdateDto
{
    public Guid? PlannerId { get; set; }
    public List<UserSelectionDto> UserSelections { get; set; } = [];
}
