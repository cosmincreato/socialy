using System.ComponentModel.DataAnnotations;

namespace DAW.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int? PostId { get; set; }
        public virtual Post? Post { get; set; }
    }
}
