using System.ComponentModel.DataAnnotations;

namespace DAW.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }
        public virtual ApplicationUser? User { get; set; }

    }
}
