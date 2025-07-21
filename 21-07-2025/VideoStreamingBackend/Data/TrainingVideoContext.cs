using Microsoft.EntityFrameworkCore;
using VideoStreamingPlatform.Models;

namespace VideoStreamingPlatform.Data;

public class TrainingVideoContext : DbContext
{
    public TrainingVideoContext(DbContextOptions<TrainingVideoContext> options) : base(options)
    {
    }
    
    public DbSet<TrainingVideo> TrainingVideos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingVideo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            entity.Property(e => e.BlobUrl)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.FileName)
                .HasMaxLength(255);
            
            entity.Property(e => e.ContentType)
                .HasMaxLength(100);
            
            entity.Property(e => e.UploadDate)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            // Indexes for better performance
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.UploadDate);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
