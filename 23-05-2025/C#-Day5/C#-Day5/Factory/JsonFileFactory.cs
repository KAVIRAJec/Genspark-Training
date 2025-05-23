using C__Day5.Concrete_Class;
using C__Day5.Inrterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Factory
{
    public class JsonFileFactory : IFileFactory
    {
            public IFileReader CreateReader()
            {
                return new JsonReader();
            }

            public IFileWriter CreateWriter()
            {
                return new JsonWriter();
            }
    }
}
