using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Exceptions
{
    public class CollectionEmptyException: Exception
    {
        string _message = "Collection is empty!";
        public CollectionEmptyException(string message) => _message = message;
        public override string Message => _message;
    }
}
