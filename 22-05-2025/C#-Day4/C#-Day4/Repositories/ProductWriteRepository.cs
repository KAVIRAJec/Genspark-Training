using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public class ProductWriteRepository: IWritableRepository<Product, int>
    {
        private static int _nextId = 100;

        public void Add(Product product)
        {
            product.Id = _nextId++;
            ProductDataStore.Products.Add(product);
        }
        public void Update(Product product)
        {
            var existing = ProductDataStore.Products.FirstOrDefault(p => p.Id == product.Id);
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
            var product = ProductDataStore.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                ProductDataStore.Products.Remove(product);
        }
    }
}
