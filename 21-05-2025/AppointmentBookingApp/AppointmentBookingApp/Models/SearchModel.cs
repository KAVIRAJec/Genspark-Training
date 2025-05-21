using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Models
{
    public class SearchModel
    {
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public AgeRange<int>? AgeRange { get; set; }
    }
    public class AgeRange<T>
    {
        public T? MinAge { get; set; }
        public T? MaxAge { get; set; }
    }
}
