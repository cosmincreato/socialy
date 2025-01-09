using System.ComponentModel.DataAnnotations.Schema;

namespace DAW.Models
{
    public class FriendRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? UserIdReceiver { get; set; }
        public string? UserIdSender { get; set; }
        public virtual ApplicationUser? Reciever { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
    }
}
