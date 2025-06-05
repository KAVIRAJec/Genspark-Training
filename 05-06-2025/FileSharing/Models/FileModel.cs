using FileApp.Models;

namespace FileApp.Models;

public class FileModel
{

    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    public string UploadedBy { get; set; } = string.Empty;
    public User? User { get; set; }
}