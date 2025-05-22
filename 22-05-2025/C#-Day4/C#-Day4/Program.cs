using C__Day4.Interfaces;
using C__Day4.Models;
using C__Day4.Repositories;
using C__Day4.Services;

namespace C__Day4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IRepository<Product,int> product = new ProductRepository();
            ProductService productService = new ProductService((ProductRepository)product);
            ManageProducts manageProducts = new ManageProducts(productService);

            IRepository<Order,int> order = new OrderRepository();
            OrderService orderService = new OrderService((OrderRepository)order,productService);
            ManageOrders manageOrders = new ManageOrders(orderService);

            //Initialize some sample products
            productService.AddProduct(new Product { Name = "Laptop", Description = "Gaming Laptop", Price = 1500, Quantity = 10, Category = "Electronics" });
            productService.AddProduct(new Product { Name = "Phone", Description = "Smartphone", Price = 800, Quantity = 20, Category = "Electronics" });
            productService.AddProduct(new Product { Name = "Headphones", Description = "Noise Cancelling", Price = 300, Quantity = 15, Category = "Electronics" });

            while (true)
            {
                Console.WriteLine("Choose an option: 1. Manage Products 2. Manage Orders 3. Exit");
                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    manageProducts.Start();
                }
                else if (choice == "2")
                {
                    manageOrders.Start();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }
}