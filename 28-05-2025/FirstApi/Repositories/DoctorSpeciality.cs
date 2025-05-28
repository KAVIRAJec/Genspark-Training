using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
    {
        protected DoctorSpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<DoctorSpeciality> Get(int key)
        {
            var speciality = await _clinicContext.DoctorSpecialities.SingleOrDefaultAsync(ds => ds.Id == key);
            return speciality ?? throw new KeyNotFoundException($"Doctor Speciality with ID {key} not found.");
        }

        public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
        {
            var specialities = await _clinicContext.DoctorSpecialities.ToListAsync();
            if (specialities == null || specialities.Count() == 0)
                throw new KeyNotFoundException("No doctor specialities in the database.");
            return specialities;
        }
    }
}