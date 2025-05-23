using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Interfaces
{
    public interface IWritableRepository<T,K>
    {
        void Add(T item);
        void Update(T item);
        void Delete(K id);
    }
}
