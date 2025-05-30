using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FAQChatBot.Contexts
{
    public class FAQContext : DbContext
    {
        public FAQContext(DbContextOptions<FAQContext> options) : base(options)
        {
        }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FAQ>().HasKey(f => f.Id);
            modelBuilder.Entity<ChatLog>().HasKey(c => c.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<FAQ>().HasOne(f => f.User).WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatLog>().HasOne(c => c.User).WithMany(u => u.ChatLogs)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatLog>().HasOne(c => c.FAQ).WithMany()
                .HasForeignKey(c => c.FAQId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}