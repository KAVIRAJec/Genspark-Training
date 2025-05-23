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
        private readonly IReadableRepository<Product, int> _readRepository;
        private readonly IWritableRepository<Product, int> _writeRepository;

        public ProductService(IReadableRepository<Product, int> readRepository, IWritableRepository<Product, int> writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        public void AddProduct(Product product)
        {
            _writeRepository.Add(product);
        }

        public Product GetProduct(int id) => _readRepository.GetById(id);

        public List<Product> GetAllProducts() => (List<Product>)_readRepository.GetAll();

        public void UpdateProduct(Product product) => _writeRepository.Update(product);

        public void DeleteProduct(int id) => _writeRepository.Delete(id);
    }
}
