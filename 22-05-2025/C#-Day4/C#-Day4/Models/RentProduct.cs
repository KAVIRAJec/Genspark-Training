using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Models
{
    public class RentProduct: Product
    {
        public int RentTimeLimitDays { get; set; }
        public string SellerName { get; set; }
        public string SellerContact { get; set; }

        public override void GetProductDetails()
        {
            base.GetProductDetails();
            Console.Write("Enter Rent Time Limit in Days: ");
            RentTimeLimitDays = int.Parse(Console.ReadLine());
            while (RentTimeLimitDays < 0)
            {
                Console.Write("Rent time limit cannot be negative. Please enter again: ");
                RentTimeLimitDays = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter Seller Name: ");
            SellerName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(SellerName))
            {
                Console.Write("Seller name cannot be empty. Please enter again: ");
                SellerName = Console.ReadLine();
            }
            Console.Write("Enter Seller Contact: ");
            SellerContact = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(SellerContact))
            {
                Console.Write("Seller contact cannot be empty. Please enter again: ");
                SellerContact = Console.ReadLine();
            }
        }
        public new string ToString()
        {
            return $"{base.ToString()} - {RentTimeLimitDays} days - {SellerName} - {SellerContact}";
        }
    }
}
