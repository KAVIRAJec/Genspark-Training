namespace BankingApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Status { get; set; } = "Active";
 
        public ICollection<Account>? Accounts { get; set; } = new List<Account>();
    }
}