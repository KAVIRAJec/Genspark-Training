namespace FileApp.Models;
public class FileModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}