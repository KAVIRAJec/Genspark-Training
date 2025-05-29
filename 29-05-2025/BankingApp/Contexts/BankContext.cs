using BankingApp.Models;
using BankingApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Contexts
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionResponseDto> TransactionResponse { get; set; }

        public async Task<TransactionResponseDto?> TransferFunds(TransferRequestDto transferDto)
        {
            var result = await this.Set<TransactionResponseDto>()
        .FromSqlInterpolated($"SELECT * FROM fn_Transfer_Funds({transferDto.AccountNumber}, {transferDto.PayeeAccountNumber}, {transferDto.Amount})")
        .ToListAsync();
        if (result == null || result.Count() == 0) return null;
        return result.FirstOrDefault();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(
                entity =>
                {
                    entity.HasKey(c => c.Id).HasName("PK_CustomerId");
                    entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                    entity.Property(c => c.PhoneNumber).HasMaxLength(15);
                });
            modelBuilder.Entity<Account>(
                entity =>
                {
                    entity.HasKey(a => a.AccountNumber).HasName("PK_AccountNumber");
                    entity.Property(a => a.Balance).HasDefaultValue(0);
                    entity.Property(a => a.AccountType).IsRequired().HasConversion<string>();
                    entity.HasOne(a => a.Customer).WithMany(c => c.Accounts)
                          .HasForeignKey(a => a.CustomerId)
                          .HasConstraintName("FK_Account_Customer")
                          .OnDelete(DeleteBehavior.Restrict);
                });
            modelBuilder.Entity<Transaction>(
                entity =>
                {
                    entity.HasKey(t => t.Id).HasName("PK_TransactionId");
                    entity.Property(t => t.TransactionType).IsRequired().HasMaxLength(50);
                    entity.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");
                    entity.Property(t => t.PayeeAccountNumber).HasMaxLength(20);

                    entity.HasOne(t => t.Account).WithMany(a => a.Transactions).HasForeignKey(t => t.AccountNumber)
                          .HasConstraintName("FK_Transaction_Account")
                          .OnDelete(DeleteBehavior.Restrict);
                }
            );
        }
    }
}