using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class GroupPost : Post
    {
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }
    }
}
