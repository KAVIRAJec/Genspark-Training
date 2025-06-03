using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FirstAPI.Models.DTOs.DoctorSpecialities
{
    public class AppointmentAddRequestDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
    }
}