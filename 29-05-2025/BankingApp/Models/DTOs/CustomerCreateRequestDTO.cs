using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models.DTOs
{
    public class CustomerCreateRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
    }
    public class CustomerUpdateRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; } = "Active";
    }
}