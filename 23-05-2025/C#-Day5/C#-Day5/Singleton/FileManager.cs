using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day5.Singleton
{
    public class FileManager
    {
        private static FileManager _instance;
        private static readonly object _lock = new object();
        private static FileStream _fileStream;

        private FileManager() { }
        public static FileManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new FileManager();
                    return _instance;
                }
            }
        }

        public void OpenFile(string path, FileMode mode)
        {
            if (_fileStream == null)
                _fileStream = new FileStream(path, mode, FileAccess.ReadWrite, FileShare.None);
        }
        public FileStream GetStream()
        {
            return _fileStream;
        }
        public void CloseFile()
        {
            _fileStream?.Close();
            _fileStream = null;
        }

    }
}
