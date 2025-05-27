using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Tweet
    {
        public int Id { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Hashtag>? Hashtags { get; set; }
    }
}