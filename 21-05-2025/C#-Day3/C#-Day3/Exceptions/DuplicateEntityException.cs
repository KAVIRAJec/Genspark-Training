using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day3.Exceptions
{
    public class DuplicateEntityException: Exception
    {
        private string _message = "Duplicate entity found!";
        public DuplicateEntityException(string message)
        {
            _message = message;
        }
        public override string Message => _message;
    }
}
