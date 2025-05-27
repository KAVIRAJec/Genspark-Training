using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int FollowerId { get; set; }
        [ForeignKey("FollowerId")]
        public User? Follower { get; set; }

        public int FollowingId { get; set; }
        [ForeignKey("FollowingId")]
        public User? Following { get; set; }
    }
}
