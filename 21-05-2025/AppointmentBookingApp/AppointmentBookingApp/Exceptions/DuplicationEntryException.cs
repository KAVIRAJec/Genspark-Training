using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Exceptions
{
    public class DuplicationEntryException: Exception
    {
        string _message = "Duplicate appointment found";
        public DuplicationEntryException(string message) => _message = message;
        public override string Message => _message;
    }
}
