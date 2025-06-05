using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment);
        public Task<Appointment> GetAppointmentByNumber(string appointmentNumber);
        public Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int patientId);
        public Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(int doctorId);
        public Task<Appointment> UpdateAppointment(string appointmentNumber, AppointmentAddRequestDto appointment);
        public Task<Appointment> DeleteAppointment(string appointmentNumber);
    }
}