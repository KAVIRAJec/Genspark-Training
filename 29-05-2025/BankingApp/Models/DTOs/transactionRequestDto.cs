using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models.DTOs
{
    public class WithdrawalRequestDto
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Withdrawal"; // Default to Withdrawal
    }
    public class DepositRequestDto
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Deposit";
    }
    public class TransferRequestDto
    {
        public string AccountNumber { get; set; }
        public string PayeeAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Transfer";
    }
}