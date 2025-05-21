using AppointmentBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Interfaces
{
    public interface IAppointmentService
    {
        int BookAppointment(Appointment appointment);
        List<Appointment> SearchAppointment(SearchModel searchModel);
    }
}
