using C__Day5.Inrterface;
using C__Day5.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace C__Day5.Concrete_Class
{
    public class JsonWriter: IFileWriter
    {
        public void Write<T>(T data)
        {
            var stream = FileManager.Instance.GetStream();
            stream.SetLength(0);
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                string json = JsonSerializer.Serialize(data);
                writer.Write(json);
                writer.Flush();
            }
        }
    }
}
