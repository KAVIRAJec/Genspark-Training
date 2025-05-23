using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProxyFileAccess.Adapter
{
    public class OldFileFormat
    {
        private readonly string _fileName;

        public OldFileFormat(string fileName)
        {
            _fileName = fileName;
        }

        public void DisplayContent()
        {
            if (System.IO.File.Exists(_fileName))
            {
                var doc = XDocument.Load(_fileName);
                Console.WriteLine("[Legacy XML] Content:");
                Console.WriteLine(doc.ToString());
            }
            else
            {
                Console.WriteLine($"File '{_fileName}' does not exist.");
            }
        }
        public void ReadMetadata()
        {
            if (System.IO.File.Exists(_fileName))
            {
                var fileInfo = new FileInfo(_fileName);
                Console.WriteLine("[Limited Access] File Metadata:");
                Console.WriteLine($"File Name: {fileInfo.Name}");
                Console.WriteLine($"File Size: {fileInfo.Length} bytes");
                Console.WriteLine($"Last Accessed: {fileInfo.LastAccessTime}");
            }
            else
            {
                Console.WriteLine("[Error] File does not exist.");
            }
        }
    }
}