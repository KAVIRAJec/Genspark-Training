using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Inrterface
{
    public interface IFileReader
    {
        T Read<T>();
    }
}
