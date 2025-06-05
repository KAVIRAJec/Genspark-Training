using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Misc
{
    public class PatientMapper
    {
    public Patient? MapPatientAddRequestPatient(PatientAddRequestDto addRequestDto)
        {
            Patient patient = new Patient();
            patient.Name = addRequestDto.Name;
            patient.Age = addRequestDto.Age;
            patient.Phone = addRequestDto.Phone;
            patient.Email = addRequestDto.Email;
            patient.Status = "Active";
            return patient;
        }        
    }
}