using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public class ProductReadRepository: IReadableRepository<Product, int>
    {
        
        public Product GetById(int id) => ProductDataStore.Products.FirstOrDefault(p => p.Id == id);

        public List<Product> GetAll() => new List<Product>(ProductDataStore.Products);

    }
}
