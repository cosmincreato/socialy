using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class UserRelationships
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Relation { get; set; }
        public string? UserId1 { get; set; }
        public string? UserId2 { get; set; }
        public virtual ApplicationUser? User1 { get; set; }
        public virtual ApplicationUser? User2 { get; set; }
    }
}
