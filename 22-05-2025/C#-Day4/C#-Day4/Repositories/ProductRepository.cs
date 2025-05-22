using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public class ProductRepository: IRepository<Product, int>
    {
        private static int _nextId = 100;
        private readonly List<Product> _products = new List<Product>();

        public void Add(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);
        }

        public Product GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public List<Product> GetAll() => _products;

        public void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Quantity = product.Quantity;
                existing.Category = product.Category;
            }
        }

        public void Delete(int id)
        {
            var product = GetById(id);
            if (product != null)
                _products.Remove(product);
        }
    }
}
