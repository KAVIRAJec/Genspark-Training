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
            IReadableRepository<Product,int> readRepo = new ProductReadRepository();
            IWritableRepository<Product,int> writeRepo = new ProductWriteRepository();
            ProductService productService = new ProductService(readRepo, writeRepo);
            ManageProducts manageProducts = new ManageProducts(productService);

            IRepository<Order,int> order = new OrderRepository();
            OrderService orderService = new OrderService((OrderRepository)order,productService);
            ManageOrders manageOrders = new ManageOrders(orderService);

            IReadableRepository<RentProduct,int> readRentRepo = new RentProductReadRepository();
            IWritableRepository<RentProduct,int> writeRentRepo = new RentProductWriteRepository();
            RentProductService rentProductService = new RentProductService(readRentRepo, writeRentRepo);
            ManageRentProducts manageRentProducts = new ManageRentProducts(rentProductService);

            //Initialize some sample products
            productService.AddProduct(new Product { Name = "Laptop", Description = "Gaming Laptop", Price = 1500, Quantity = 10, Category = "Electronics" });
            productService.AddProduct(new Product { Name = "Phone", Description = "Smartphone", Price = 800, Quantity = 20, Category = "Electronics" });
            productService.AddProduct(new Product { Name = "Headphones", Description = "Noise Cancelling", Price = 300, Quantity = 15, Category = "Electronics" });
            // productService.AddProduct(new RentProduct { Name = "Projector", Description = "HD Projector", Price = 200, Quantity = 2, Category = "Electronics", RentTimeLimitDays = 5, SellerName = "Alice", SellerContact = "555-1234" });

            rentProductService.AddRentProduct(new RentProduct { Name = "Camera", Description = "DSLR Camera", Price = 500, Quantity = 5, Category = "Electronics", RentTimeLimitDays = 7, SellerName = "John Doe", SellerContact = "123-456-7890" });
            rentProductService.AddRentProduct(new RentProduct { Name = "Drone", Description = "Quadcopter Drone", Price = 700, Quantity = 3, Category = "Electronics", RentTimeLimitDays = 14, SellerName = "Jane Smith", SellerContact = "987-654-3210" });
            while (true)
            {
                Console.WriteLine("Choose an option: 1. Manage Products 2. Manage Orders 3. Manage Rent Products 4. Exit");
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
                    manageRentProducts.Start();
                }
                else if (choice == "4")
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