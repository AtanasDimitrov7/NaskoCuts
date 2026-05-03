using Microsoft.AspNetCore.Identity;

namespace NaskoCuts.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}