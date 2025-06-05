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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileModel>().HasKey(f => f.Id);
        modelBuilder.Entity<FileModel>().Property(f => f.FileName).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<FileModel>().Property(f => f.FileType).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<FileModel>().Property(f => f.Size).IsRequired();
        modelBuilder.Entity<FileModel>().Property(f => f.CreatedDate).IsRequired();
    }
}