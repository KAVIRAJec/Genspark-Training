using C__Day4.Interfaces;
using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Services
{
    public class RentProductService
    {
        private readonly IReadableRepository<RentProduct, int> _readRepo;
        private readonly IWritableRepository<RentProduct, int> _writeRepo;

        public RentProductService(IReadableRepository<RentProduct, int> readRepo, IWritableRepository<RentProduct, int> writeRepo)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
        }

        public void AddRentProduct(RentProduct rentProduct) => _writeRepo.Add(rentProduct);
        public RentProduct GetRentProduct(int id) => _readRepo.GetById(id);
        public List<RentProduct> GetAllRentProducts() => _readRepo.GetAll();
        public void UpdateRentProduct(RentProduct rentProduct) => _writeRepo.Update(rentProduct);
        public void DeleteRentProduct(int id) => _writeRepo.Delete(id);
    }
}
