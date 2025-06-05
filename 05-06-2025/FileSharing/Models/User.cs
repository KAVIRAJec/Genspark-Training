namespace FileApp.Models;

public class User
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[]? Password { get; set; }
    public byte[]? HashKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = string.Empty; // e.g., "Admin", "User"

    // Navigation property for files uploaded by the user
    public ICollection<FileModel>? UploadedFiles { get; set; } = new List<FileModel>();
}