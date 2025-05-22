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
    public class ProductService
    {
        private readonly IRepository<Product,int> _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        public void AddProduct(Product product)
        {
            _repository.Add(product);
        }

        public Product GetProduct(int id) => _repository.GetById(id);

        public List<Product> GetAllProducts() => (List<Product>)_repository.GetAll();

        public void UpdateProduct(Product product) => _repository.Update(product);

        public void DeleteProduct(int id) => _repository.Delete(id);
    }
}
