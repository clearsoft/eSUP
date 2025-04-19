namespace eSUP.DTO;

public class UserInformationDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Confirmed { get; set; }
    public string? Role { get; set; }
    public bool IsAssigned { get; set; }
}
