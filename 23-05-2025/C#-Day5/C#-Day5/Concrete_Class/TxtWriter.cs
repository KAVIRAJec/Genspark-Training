using C__Day5.Inrterface;
using C__Day5.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Concrete_Class
{
    public class TxtWriter: IFileWriter
    {
        public void Write<T>(T content)
        {
            var stream = FileManager.Instance.GetStream();
            stream.Seek(0, SeekOrigin.End);
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.Write(content);
                writer.Flush();
            }
        }
    }
}
