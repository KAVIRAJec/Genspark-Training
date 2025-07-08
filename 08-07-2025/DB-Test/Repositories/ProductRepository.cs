using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DBTest.Data;
using DBTest.Interfaces;
using DBTest.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest.Repositories
{
    public class ProductRepository : IRepository<Product, int>
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository()
        {
            _context = new ApplicationDbContext();
        }

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public Product Add(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Rethrow to be handled by the controller
            }
        }

        // READ
        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public IEnumerable<Product> Find(Expression<Func<Product, bool>> predicate)
        {
            return _context.Products.Where(predicate).ToList();
        }

        // UPDATE
        public Product Update(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return product;
        }

        // DELETE
        public bool Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
    }
}