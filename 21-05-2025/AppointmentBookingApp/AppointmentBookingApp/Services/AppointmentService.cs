using AppointmentBookingApp.Interfaces;
using AppointmentBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        IRepository<int, Appointment> _appointmentRepository;
        public AppointmentService(IRepository<int, Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public int BookAppointment(Appointment appointment)
        {
            try
            {
                var result = _appointmentRepository.Create(appointment);
                if (result != null)
                {
                    return result.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return -1;
        }
        public List<Appointment> SearchAppointment(SearchModel searchModel)
        {
            try
            {
                var appointments = _appointmentRepository.GetAll();
                appointments = SearchByName(appointments, searchModel.PatientName);
                appointments = SearchByDate(appointments, searchModel.AppointmentDate);
                appointments = SearchByAge(appointments, searchModel.AgeRange);
                if (appointments != null && appointments.Count > 0)
                {
                    return appointments.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null;
        }
        private ICollection<Appointment> SearchByName(ICollection<Appointment> appointments, string? PatientName)
        {
            if (PatientName == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientName.ToLower().Contains(PatientName.ToLower())).ToList();
            }
        }
        private ICollection<Appointment> SearchByDate(ICollection<Appointment> appointments, DateTime? AppointmentDate)
        {
            Console.WriteLine(AppointmentDate);
            if (AppointmentDate == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.AppointmentDate == AppointmentDate).ToList();
            }
        }
        private ICollection<Appointment> SearchByAge(ICollection<Appointment> appointments, AgeRange<int>? ageRange)
        {
            if (ageRange == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientAge >= ageRange.MinAge && a.PatientAge <= ageRange.MaxAge).ToList();
            }
        }
    }
}
