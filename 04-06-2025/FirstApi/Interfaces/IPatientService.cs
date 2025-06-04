using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Interfaces
{
    public interface IPatientService
    {
        public Task<Patient> GetPatientByName(string name);
        public Task<Patient> AddPatient(PatientAddRequestDto patient);
    }
}