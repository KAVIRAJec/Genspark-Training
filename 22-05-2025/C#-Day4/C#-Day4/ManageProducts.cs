using C__Day4.Models;
using C__Day4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4
{
    public class ManageProducts
    {
        private readonly ProductService _service;

        public ManageProducts(ProductService service)
        {
            _service = service;
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("\n--- Product Management ---");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. View All Products");
                Console.WriteLine("3. Get Product By Id");
                Console.WriteLine("4. Update Product");
                Console.WriteLine("5. Delete Product");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        ViewAllProducts();
                        break;
                    case "3":
                        GetProductById();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        DeleteProduct();
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

        private void AddProduct()
        {
            var product = new Product();
            product.GetProductDetails();
            _service.AddProduct(product);
            Console.WriteLine("Product added successfully.");
        }
        private void ViewAllProducts()
        {
            var products = _service.GetAllProducts();
            if (products == null || products?.Count() == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }
            foreach (var p in products)
            {
                Console.WriteLine(p.ToString());
            }
        }
        private void GetProductById()
        {
            Console.Write("Enter Product Id: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var product = _service.GetProduct(id);
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                }
                else
                {
                    Console.WriteLine(product.ToString());
                }
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }
        private void UpdateProduct()
        {
            Console.Write("Enter Product Id to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var product = _service.GetProduct(id);
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    return;
                }
                product.GetProductDetails();
                _service.UpdateProduct(product);
                Console.WriteLine("Product updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }
        private void DeleteProduct()
        {
            Console.Write("Enter Product Id to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _service.DeleteProduct(id);
                Console.WriteLine("Product deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }
    }
}
