using C__Day4.Models;
using C__Day4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4
{
    public class ManageRentProducts
    {
        private readonly RentProductService _rentProductService;
        public ManageRentProducts(RentProductService rentProductService)
        {
            _rentProductService = rentProductService;
        }
        public void Start()
        {
            while (true)
            {
                Console.WriteLine("\n--- Rent Product Management ---");
                Console.WriteLine("1. Add Rent Product");
                Console.WriteLine("2. View All Rent Products");
                Console.WriteLine("3. Get Rent Product By Id");
                Console.WriteLine("4. Update Rent Product");
                Console.WriteLine("5. Delete Rent Product");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddRentProduct();
                        break;
                    case "2":
                        ViewAllRentProducts();
                        break;
                    case "3":
                        GetRentProductById();
                        break;
                    case "4":
                        UpdateRentProduct();
                        break;
                    case "5":
                        DeleteRentProduct();
                        break;
                    case "6":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        private void AddRentProduct()
        {
            var rentProduct = new RentProduct();
            rentProduct.GetProductDetails();
            _rentProductService.AddRentProduct(rentProduct);
            Console.WriteLine("Rent Product added successfully.");
        }
        private void ViewAllRentProducts()
        {
            var rentProducts = _rentProductService.GetAllRentProducts();
            if (rentProducts == null || rentProducts.Count == 0)
            {
                Console.WriteLine("No rent products found.");
                return;
            }
            Console.WriteLine("\n--- All Rent Products ---");
            foreach (var r in rentProducts)
            {
                Console.WriteLine(r.ToString());
            }
        }
        private void GetRentProductById()
        {
            Console.Write("Enter Rent Product ID: ");
            int id = int.Parse(Console.ReadLine());
            var rentProduct = _rentProductService.GetRentProduct(id);
            if (rentProduct == null)
            {
                Console.WriteLine("Rent Product not found.");
                return;
            }
            Console.WriteLine("\n--- Rent Product Details ---");
            Console.WriteLine(rentProduct.ToString());
        }
        private void UpdateRentProduct()
        {
            Console.Write("Enter Rent Product ID to update: ");
            int id = int.Parse(Console.ReadLine());
            var rentProduct = _rentProductService.GetRentProduct(id);
            if (rentProduct == null)
            {
                Console.WriteLine("Rent Product not found.");
                return;
            }
            rentProduct.GetProductDetails();
            _rentProductService.UpdateRentProduct(rentProduct);
            Console.WriteLine("Rent Product updated successfully.");
        }
        private void DeleteRentProduct()
        {
            Console.Write("Enter Rent Product ID to delete: ");
            int id = int.Parse(Console.ReadLine());
            var rentProduct = _rentProductService.GetRentProduct(id);
            if (rentProduct == null)
            {
                Console.WriteLine("Rent Product not found.");
                return;
            }
            _rentProductService.DeleteRentProduct(id);
            Console.WriteLine("Rent Product deleted successfully.");
        }
    }
}
