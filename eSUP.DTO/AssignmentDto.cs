namespace eSUP.DTO;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public List<UserInformationDto> Users { get; set; } = [];
}
