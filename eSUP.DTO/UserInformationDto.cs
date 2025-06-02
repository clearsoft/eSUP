using CsvHelper.Configuration.Attributes;

namespace eSUP.DTO;

public class UserInformationDto
{
    [Ignore] public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Group { get; set; }
    [Ignore] public bool Confirmed { get; set; }
    [Ignore] public string? Role { get; set; }
    [Ignore] public bool IsAssigned { get; set; }
    [Ignore]
    public bool IsSelected { get; set; }
}
