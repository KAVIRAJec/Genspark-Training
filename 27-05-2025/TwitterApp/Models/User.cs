namespace TwitterApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Tweet>? Tweets { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Hashtag>? Hashtags { get; set; }
        public ICollection<Follow>? Followers { get; set; }
        public ICollection<Follow>? Following { get; set; }
    }
}