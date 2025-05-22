using C__Day4.Interfaces;
using C__Day4.Models;
using C__Day4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public class OrderRepository: IRepository<Order, int>
    {
        private readonly List<Order> _orders = new List<Order>();
        private int _nextId = 200;

        public void Add(Order order)
        {
            order.Id = _nextId++;
            _orders.Add(order);
        }

        public Order GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

        public List<Order> GetAll() => _orders;

        public void Update(Order order)
        {
            var existing = GetById(order.Id);
            if (existing != null)
            {
                existing.ProductId = order.ProductId;
                existing.Quantity = order.Quantity;
                existing.OrderDate = order.OrderDate;
                existing.CustomerName = order.CustomerName;
            }
        }

        public void Delete(int id)
        {
            var order = GetById(id);
            if (order != null)
                _orders.Remove(order);
        }
    }
}
