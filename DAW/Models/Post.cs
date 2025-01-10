using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAW.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "You can't submit an empty post")]
        public string? Content { get; set; }
        public string? Label { get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string? Image {  get; set; }
        public string? Video { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

    }
}
