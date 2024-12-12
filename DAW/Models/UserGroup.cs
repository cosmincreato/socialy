using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class UserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id;

        public string? UserId {  get; set; }
        public int? GroupId { get; set; }
        
        public virtual ApplicationUser? User { get; set; }
        public virtual Group? Group { get; set; }
    }
}
