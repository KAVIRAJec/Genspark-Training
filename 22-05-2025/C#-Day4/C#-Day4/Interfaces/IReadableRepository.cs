using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Interfaces
{
    public interface IReadableRepository<T, K>
    {
        T GetById(K id);
        List<T> GetAll();
    }
}
