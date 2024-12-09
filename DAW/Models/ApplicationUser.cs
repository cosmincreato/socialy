using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAW.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Bio {  get; set; }

        //[Required(ErrorMessage = "First name required")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "You must choose a visibility option")]
        public bool IsPublic { get; set; }

        public ICollection<Post>? Posts { get; set; }
        
        public ICollection<GroupPost>? GroupPosts { get; set; }
    }
}
