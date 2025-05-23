using ProxyFileAccess.Adapter;
using ProxyFileAccess.Interfaces;
using ProxyFileAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFileAccess
{
    public class ProxyFile : IFile
    {
        //private readonly File _realFile = new File("C:\\Users\\kaviraja\\Downloads\\Genspark-Training\\23-05-2025\\ProxyFileAccess\\ProxyFileAccess\\Files\\secret.txt");
        //private readonly File _realFile = new File("C:\\Users\\kaviraja\\Downloads\\Genspark-Training\\23-05-2025\\ProxyFileAccess\\ProxyFileAccess\\Files\\secret.json");
        private readonly Adaptor _realFile = new Adaptor("C:\\Users\\kaviraja\\Downloads\\Genspark-Training\\23-05-2025\\ProxyFileAccess\\ProxyFileAccess\\Files\\secret.xml");
        private readonly User _user;
        public ProxyFile( User user)
        {
            _user = user;
        }
        public void Read()
        {
            if (_user == null)
            {
                Console.WriteLine("[Access Denied] Invalid user.");
                return;
            }

            switch (_user.Role.ToString().ToLower())
            {
                case "admin":
                    _realFile.Read();
                    break;
                case "user":
                    _realFile.ReadMetadata();
                    break;
                case "guest":
                default:
                    Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                    break;
            }
        }
    }
}
