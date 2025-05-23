using C__Day5.Inrterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Factory
{
    public interface IFileFactory
    {
        IFileReader CreateReader();
        IFileWriter CreateWriter();
    }
}
