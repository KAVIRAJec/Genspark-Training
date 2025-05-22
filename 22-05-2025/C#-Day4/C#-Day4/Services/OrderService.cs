using C__Day4.Interfaces;
using C__Day4.Models;
using C__Day4.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Services
{
    public class OrderService
    {
        private readonly IRepository<Order,int> _repo;
        private readonly ProductService _productService;

        public OrderService(OrderRepository repo, ProductService productService)
        {
            _repo = repo;
            _productService = productService;
        }

        public void AddOrder(Order order)
        {
            Product product = _productService.GetProduct(order.ProductId);
            if (product == null)
            {
                Console.WriteLine("Product Id not found!");
            }
            if (product?.Quantity < order.Quantity)
            {
                Console.WriteLine("Insufficient product quantity!");
            }
            product.Quantity -= order.Quantity;
            _productService.UpdateProduct(product);
            _repo.Add(order);
        }

        public Order GetOrder(int id) => _repo.GetById(id);

        public List<Order> GetAllOrders() => _repo.GetAll();

        public void UpdateOrder(Order order) => _repo.Update(order);

        public void DeleteOrder(int id) => _repo.Delete(id);
    }
}
