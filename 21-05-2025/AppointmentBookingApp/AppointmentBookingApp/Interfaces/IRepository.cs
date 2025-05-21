using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Interfaces
{
    public interface IRepository<K,T> where T: class
    {
        T Create(T item);
        T GetById(K id);
        ICollection<T> GetAll();
        T Update(T item);
        T Delete(K id);

    }
}
