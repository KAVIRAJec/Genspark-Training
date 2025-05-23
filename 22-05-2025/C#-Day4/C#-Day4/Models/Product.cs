using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public virtual void GetProductDetails()
        {
            Console.Write("Enter Product Name: ");
            Name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(Name))
            {
                Console.Write("Product name cannot be empty. Please enter again: ");
                Name = Console.ReadLine();
            }
            Console.Write("Enter Description: ");
            Description = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(Description))
            {
                Console.Write("Description cannot be empty. Please enter again: ");
                Description = Console.ReadLine();
            }
            Console.Write("Enter Price: ");
            Price = decimal.Parse(Console.ReadLine());
            while (Price < 0)
            {
                Console.Write("Price cannot be negative. Please enter again: ");
                Price = decimal.Parse(Console.ReadLine());
            }
            Console.Write("Enter Quantity: ");
            Quantity = int.Parse(Console.ReadLine());
            while (Quantity < 0)
            {
                Console.Write("Quantity cannot be negative. Please enter again: ");
                Quantity = int.Parse(Console.ReadLine());
            }
            Console.Write("Enter Category: ");
            Category = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(Category))
            {
                Console.Write("Category cannot be empty. Please enter again: ");
                Category = Console.ReadLine();
            }
        }
        public string ToString()
        {
            return $"{Id} - {Name} - {Description} - Rs.{Price} - {Quantity} - {Category}";
        }
    }
}
