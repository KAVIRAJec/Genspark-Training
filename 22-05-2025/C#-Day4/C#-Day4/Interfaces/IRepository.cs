using C__Day4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day4.Interfaces
{
    public interface IRepository<T, K> : IReadableRepository<T, K>, IWritableRepository<T, K>
    {
    }
}
