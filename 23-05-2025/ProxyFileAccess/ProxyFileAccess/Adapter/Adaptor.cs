using ProxyFileAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyFileAccess.Adapter
{
    public class Adaptor: IFile
    {
        private readonly OldFileFormat _file;

        public Adaptor(string fileName)
        {
            _file = new OldFileFormat(fileName);
        }

        public void Read()
        {
            _file.DisplayContent();
        }
        public void ReadMetadata()
        {
            _file.ReadMetadata();
        }
    }
}
