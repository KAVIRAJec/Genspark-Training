using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;


namespace FirstAPI.Misc
{
    public class OtherFunctionalitiesImplementation : IOtherContextFunctionalities
    {
        private readonly ClinicContext _clinicContext;

        public OtherFunctionalitiesImplementation(ClinicContext clinicContext)
        {
            _clinicContext = clinicContext;
        }

        public virtual async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            try
            {
                if (string.IsNullOrEmpty(speciality))
                {
                    throw new ArgumentException("Speciality cannot be null or empty", nameof(speciality));
                }
                if (speciality.Length < 3 || speciality.Length > 50)
                {
                    throw new ArgumentException("Speciality must be between 3 and 50 characters long", nameof(speciality));
                }
                var result = await _clinicContext.GetDoctorsBySpeciality(speciality);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching doctors by speciality", ex);
            }
        }
    }
}