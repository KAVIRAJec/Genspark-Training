using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Interfaces
{
    public interface IRepository<T, K> where T : class
    {
        void Add(T item);
        T GetById(K id);
        List<T> GetAll();
        void Update(T item);
        void Delete(K id);
    }
}
