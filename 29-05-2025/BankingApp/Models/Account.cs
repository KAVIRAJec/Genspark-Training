namespace BankingApp.Models
{
    public class Account
    {
        public string AccountNumber { get; set; } = Guid.NewGuid().ToString();
        public decimal Balance { get; set; }
        public BAccountType AccountType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}

public enum BAccountType
{
    Saving,
    Student,
    Business,
    Loan
}