using ProxyFileAccess.Interfaces;
using System.Text.Json;

namespace ProxyFileAccess
{
    public class File : IFile
    {
        private readonly string _fileName;

        public File(string fileName)
        {
            _fileName = fileName;
        }
        public void Read()
        {
            if (System.IO.File.Exists(_fileName))
            {
                string content = System.IO.File.ReadAllText(_fileName);
                Console.WriteLine($"[Access Granted] Reading sensitive file content:");
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty.");
                    return;
                }
                try
                {
                    var doc = JsonDocument.Parse(content);
                    foreach (var item in doc.RootElement.EnumerateArray())
                    {
                        Console.WriteLine("---- Entry ----");
                        Console.WriteLine($"Title: {item.GetProperty("Title")}");
                        Console.WriteLine($"Author: {item.GetProperty("Author")}");
                        Console.WriteLine($"Date: {item.GetProperty("Date")}");
                        Console.WriteLine($"Summary: {item.GetProperty("Summary")}");
                        Console.WriteLine($"Content: {item.GetProperty("Content")}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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

                try
                {
                    string content = System.IO.File.ReadAllText(_fileName);
                    if (string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine("Total Entries: 0 (File is empty)");
                        return;
                    }

                    var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        int count = doc.RootElement.GetArrayLength();
                        Console.WriteLine($"Total Entries: {count}");
                    }
                    else
                    {
                        Console.WriteLine("Total Entries: N/A (Not a JSON array)");
                    }
                }
                catch
                {
                    Console.WriteLine("[Error] Unable to parse file metadata.");
                }
            }
            else
            {
                Console.WriteLine("[Error] File does not exist.");
            }
        }
    }
}
