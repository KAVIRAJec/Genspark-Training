using FileApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FileApp.Contexts;
public class FileAppContext : DbContext
{
    public FileAppContext(DbContextOptions<FileAppContext> options) : base(options)
    {
    }

    public DbSet<FileModel> Files { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileModel>().HasKey(f => f.Id);
        modelBuilder.Entity<FileModel>().HasOne(f => f.User)
                                        .WithMany(u => u.UploadedFiles)
                                        .HasForeignKey(f => f.UploadedBy)
                                        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>().HasKey(u => u.UserName);
    }
}