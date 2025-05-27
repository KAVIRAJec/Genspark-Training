namespace TwitterApp.Models
{
    public class Hashtag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Tweet>? Tweets { get; set; }
    }
}