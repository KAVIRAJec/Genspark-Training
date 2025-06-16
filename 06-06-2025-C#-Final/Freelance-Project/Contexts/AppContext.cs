using Microsoft.EntityFrameworkCore;
using Freelance_Project.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Freelance_Project.Contexts;

public class FreelanceDBContext : DbContext
{
    public FreelanceDBContext(DbContextOptions<FreelanceDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Freelancer> Freelancers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.HashKey).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            entity.Property(u => u.CreatedAt).IsRequired();
        });
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.Email)
                .HasConstraintName("FK_Client_User")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(c => c.Projects)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Freelancer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ExperienceYears).IsRequired();
            entity.Property(e => e.HourlyRate).IsRequired().HasColumnType("decimal(18,2)");
            entity.HasOne(u => u.User)
                .WithOne(f => f.Freelancer)
                .HasForeignKey<Freelancer>(f => f.Email)
                .HasConstraintName("FK_Freelancer_User")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(f => f.Skills)// Many-to-many relationship with Skills
                .WithMany(s => s.Freelancers)
                .UsingEntity(j => j.ToTable("FreelancerSkills"));
            entity.HasMany(f => f.Proposals)
                .WithOne(p => p.Freelancer)
                .HasForeignKey(p => p.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Budget).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.HasMany(p => p.RequiredSkills)/// Many-to-many relationship with Skills
                .WithMany(s => s.Projects)
                .UsingEntity(j => j.ToTable("ProjectSkills"));
            entity.HasMany(p => p.Proposals)
                .WithOne(pr => pr.Project)
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProposedAmount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasOne(pr => pr.Freelancer)
                .WithMany(f => f.Proposals)
                .HasForeignKey(pr => pr.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(pr => pr.Project)
                .WithMany(p => p.Proposals)
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasOne(cr => cr.Client)
                .WithMany(c => c.ChatRooms)
                .HasForeignKey(cr => cr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(cr => cr.Freelancer)
                .WithMany(f => f.ChatRooms)
                .HasForeignKey(cr => cr.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(cr => cr.Messages)
                .WithOne(cm => cm.ChatRoom)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SentAt).IsRequired();
            entity.HasOne(cm => cm.ChatRoom)
                .WithMany(cr => cr.Messages)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}