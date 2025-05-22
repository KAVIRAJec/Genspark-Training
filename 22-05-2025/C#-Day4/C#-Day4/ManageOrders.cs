using C__Day4.Models;
using C__Day4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4
{
    public class ManageOrders
    {
        private readonly OrderService _service;

        public ManageOrders(OrderService service)
        {
            _service = service;
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("\n--- Order Management ---");
                Console.WriteLine("1. Book Order");
                Console.WriteLine("2. View All Orders");
                Console.WriteLine("3. Get Order By Id");
                Console.WriteLine("4. Update Order");
                Console.WriteLine("5. Delete Order");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddOrder();
                        break;
                    case "2":
                        ViewAllOrders();
                        break;
                    case "3":
                        GetOrderById();
                        break;
                    case "4":
                        UpdateOrder();
                        break;
                    case "5":
                        DeleteOrder();
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

        private void AddOrder()
        {
            var order = new Order();
            order.GetOrderDetails();
            order.OrderDate = DateTime.Now;
            _service.AddOrder(order);
            Console.WriteLine("Order added successfully.");
        }

        private void ViewAllOrders()
        {
            var orders = _service.GetAllOrders();
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }
            foreach (var o in orders)
            {
                Console.WriteLine(o.ToString());
            }
        }

        private void GetOrderById()
        {
            Console.Write("Enter Order Id: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var order = _service.GetOrder(id);
                if (order == null)
                {
                    Console.WriteLine("Order not found.");
                }
                else
                {
                    Console.WriteLine(order.ToString());
                }
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }

        private void UpdateOrder()
        {
            Console.Write("Enter Order Id to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var order = _service.GetOrder(id);
                if (order == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }
                Console.Write("Enter New Product Id: ");
                order.ProductId = int.Parse(Console.ReadLine());
                Console.Write("Enter New Quantity: ");
                order.Quantity = int.Parse(Console.ReadLine());
                Console.Write("Enter New Customer Name: ");
                order.CustomerName = Console.ReadLine();
                order.OrderDate = DateTime.Now;
                _service.UpdateOrder(order);
                Console.WriteLine("Order updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }

        private void DeleteOrder()
        {
            Console.Write("Enter Order Id to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _service.DeleteOrder(id);
                Console.WriteLine("Order deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid Id.");
            }
        }
    }
}
