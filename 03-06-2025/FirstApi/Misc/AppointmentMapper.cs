using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Misc
{
    public class AppointmentMapper
    {
    public Appointment? MapAppointmentAddRequestDto(AppointmentAddRequestDto addRequestDto)
        {
            Appointment appointment = new();
            appointment.AppointmentNumber = Guid.NewGuid().ToString();
            appointment.PatientId = addRequestDto.PatientId;
            appointment.DoctorId = addRequestDto.DoctorId;
            appointment.AppointmentDateTime = addRequestDto.AppointmentDateTime;
            appointment.Status = "Active"; // Active, "Cancelled", "Completed"
            return appointment;
        }        
    }
}