using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int TweetId { get; set; }
        [ForeignKey("TweetId")]
        public Tweet? Tweet { get; set; }
    }
}