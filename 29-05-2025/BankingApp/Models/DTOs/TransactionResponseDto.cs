using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingApp.Models.DTOs
{
    public class TransactionResponseDto
    {
        [Column("sender_account")]
        [Key]
        public string AccountNumber { get; set; }
        [Column("receiver_account")]
        public string PayeeAccountNumber { get; set; }
        [Column("transferred_amount")]
        public decimal Amount { get; set; }
        [Column("transaction_type")]
        public string TransactionType { get; set; } = "Withdrawal";
    }
}