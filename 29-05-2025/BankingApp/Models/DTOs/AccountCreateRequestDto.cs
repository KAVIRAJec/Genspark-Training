using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models.DTOs
{
    public class AccountCreateRequestDto
    {
        public int CustomerId { get; set; }
        public decimal InitialBalance { get; set; }
        public string AccountType { get; set; } // Saving, Student, Business, Loan
    }
}