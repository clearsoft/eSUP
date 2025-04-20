using eSUP.Client.Pages;
using Microsoft.AspNetCore.Identity;

namespace eSUP.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Group { get; set; }
        public List<Planner> Planners { get; set; } = [];
        public List<Part> Parts { get; set; } = [];
    }

}
