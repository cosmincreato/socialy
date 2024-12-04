using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class GroupPost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? GroupId { get; set; }

        public string? UserId { get; set; }

        public virtual Group? Group { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
