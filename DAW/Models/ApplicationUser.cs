using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string? ProfilePicture { get; set; } = "/images/no-picture.png";

        public ICollection<Post>? Posts { get; set; }
        
        public ICollection<GroupPost>? GroupPosts { get; set; }

        public virtual ICollection<UserGroup>? UserGroups { get; set; }
        
        //pentru posturi
        [NotMapped]
        public string? Content { get; set; }
        [NotMapped]
        public string? Image { get; set; }
        [NotMapped]
        public string? Video { get; set; }
    }
}
