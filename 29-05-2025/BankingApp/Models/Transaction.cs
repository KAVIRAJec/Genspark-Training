namespace BankingApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } // e.g., Deposit, Withdrawal, Transfer
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string AccountNumber { get; set; }
        public Account Account { get; set; }
        
        public string? PayeeAccountNumber { get; set; } // For transfers
        public Account? PayeeAccount { get; set; }
    }
}
