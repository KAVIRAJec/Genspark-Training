using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Repositories
{
    public static class RentProductDataStore
    {
        public static List<RentProduct> RentProducts { get; } = new List<RentProduct>();
    }

    public class RentProductReadRepository : IReadableRepository<RentProduct, int>
    {
        public RentProduct GetById(int id) => RentProductDataStore.RentProducts.FirstOrDefault(r => r.Id == id);
        public List<RentProduct> GetAll() => new List<RentProduct>(RentProductDataStore.RentProducts);
    }

    public class RentProductWriteRepository : IWritableRepository<RentProduct, int>
    {
        private static int _nextId = 1000;
        public void Add(RentProduct rentProduct)
        {
            rentProduct.Id = _nextId++;
            RentProductDataStore.RentProducts.Add(rentProduct);
        }
        public void Update(RentProduct rentProduct)
        {
            var existing = GetById(rentProduct.Id);
            if (existing != null)
            {
                existing.Name = rentProduct.Name;
                existing.Description = rentProduct.Description;
                existing.Price = rentProduct.Price;
                existing.Quantity = rentProduct.Quantity;
                existing.Category = rentProduct.Category;
                existing.RentTimeLimitDays = rentProduct.RentTimeLimitDays;
                existing.SellerName = rentProduct.SellerName;
                existing.SellerContact = rentProduct.SellerContact;
            }
        }
        public void Delete(int id)
        {
            var rentProduct = GetById(id);
            if (rentProduct != null)
                RentProductDataStore.RentProducts.Remove(rentProduct);
        }
        private RentProduct GetById(int id) => RentProductDataStore.RentProducts.FirstOrDefault(r => r.Id == id);
    }
}
