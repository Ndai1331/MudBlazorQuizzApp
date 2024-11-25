using Microsoft.AspNetCore.Identity;

namespace QuizAppBlazor.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
        public string Role { get; set; } = "User";
    }
}
