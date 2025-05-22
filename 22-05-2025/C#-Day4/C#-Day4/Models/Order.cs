using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }

        public void GetOrderDetails()
        {
            Console.Write("Enter Product Id: ");
            ProductId = int.Parse(Console.ReadLine());
            while(ProductId <= 0)
            {
                Console.Write("Product Id must be greater than 0. Please enter again: ");
                ProductId = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter Quantity: ");
            Quantity = int.Parse(Console.ReadLine());
            while(Quantity <= 0)
            {
                Console.Write("Quantity must be greater than 0. Please enter again: ");
                Quantity = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter your Name: ");
            CustomerName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(CustomerName))
            {
                Console.Write("Customer name cannot be empty. Please enter again: ");
                CustomerName = Console.ReadLine();
            }
        }

        public override string ToString()
        {
            return $"{Id} - ProductId: {ProductId} - Qty: {Quantity} - Date: {OrderDate:d} - Customer: {CustomerName}";
        }
    }
}
