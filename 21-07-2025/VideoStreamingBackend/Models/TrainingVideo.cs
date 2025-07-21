using System.ComponentModel.DataAnnotations;

namespace VideoStreamingPlatform.Models;

public class TrainingVideo
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime UploadDate { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string BlobUrl { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? FileName { get; set; }
    
    public long FileSize { get; set; }
    
    [MaxLength(100)]
    public string? ContentType { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
