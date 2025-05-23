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
    public class JsonReader: IFileReader
    {
        public T Read<T>()
        {
            var stream = FileManager.Instance.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                string json = reader.ReadToEnd();
                if (string.IsNullOrEmpty(json))
                    return default(T);
                return JsonSerializer.Deserialize<T>(json);
            }
        }
    }
}
