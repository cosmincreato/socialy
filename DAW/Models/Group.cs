using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Groups must have a name")]
        [MinLength(5, ErrorMessage ="Group names must contain at least 5 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage ="Groups must have a description")]
        public string? Description { get; set; }

        public string? Label { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<GroupPost>? Posts { get; set; }
      
        public virtual ICollection<UserGroup>? UserGroups { get; set; }

        [NotMapped]
        public string? Content {  get; set; }
    }
}
