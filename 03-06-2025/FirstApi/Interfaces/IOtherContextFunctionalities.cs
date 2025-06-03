using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Interfaces
{
    public interface IOtherContextFunctionalities
    {
        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality);
    }
}