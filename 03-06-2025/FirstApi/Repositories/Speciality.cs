using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class SpecialityRepository : Repository<int, Speciality>
    {
        public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Speciality> Get(int key)
        {
            var speciality = await _clinicContext.Specialities.
                                                  Include(s => s.DoctorSpecialities).
                                                  SingleOrDefaultAsync(s => s.Id == key);
            return speciality ?? throw new KeyNotFoundException($"Speciality with ID {key} not found.");
        }

        public override async Task<IEnumerable<Speciality>> GetAll()
        {
            var specialities = await _clinicContext.Specialities.
                                                  Include(s => s.DoctorSpecialities).
                                                  ToListAsync();
            if (specialities == null || specialities.Count() == 0)
                throw new KeyNotFoundException("No specialities in the database.");
            return specialities;
        }
    }
}