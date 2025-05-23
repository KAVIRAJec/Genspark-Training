using C__Day5.Inrterface;
using C__Day5.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Concrete_Class
{
    public class TxtReader: IFileReader
    {
        public T Read<T>()
        {
            var stream = FileManager.Instance.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                string content = reader.ReadToEnd();
                return (T)Convert.ChangeType(content, typeof(T));
            }
        }
    }
}
