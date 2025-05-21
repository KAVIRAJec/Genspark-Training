using AppointmentBookingApp.Exceptions;
using AppointmentBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Repositories
{
    public class AppointmentRepository: BaseRepository<int, Appointment> //Concreate class
    {
        public AppointmentRepository(): base()
        {
        }
        public override int GenerateID()
        {
            if (_items.Count == 0) return 101;
            return _items.Max(a => a.Id) + 1;
        }
        public override Appointment GetById(int id)
        {
            var appointment = _items.FirstOrDefault(a => a.Id == id);
            if (appointment == null) throw new KeyNotFoundException("Appointment not found.");
            return appointment;
        }
        public override ICollection<Appointment> GetAll()
        {
            if(_items.Count == 0) throw new CollectionEmptyException("No Appointments Found.");
            return _items;
        }
    }
}
