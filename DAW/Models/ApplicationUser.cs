using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DAW.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required(ErrorMessage = "Bio required")]
        public string Bio { get; set; } = "Hi! I'm using Socialy!";

        [Required(ErrorMessage = "First name required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; }

        public bool IsPublic { get; set; } = true;

        public string? ProfilePicture { get; set; }

        public ICollection<Post>? Posts { get; set; }
        
        public ICollection<GroupPost>? GroupPosts { get; set; }

        public virtual ICollection<UserGroup>? UserGroups { get; set; }
    }
}
