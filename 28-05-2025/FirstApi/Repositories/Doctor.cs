using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class DoctorRepository : Repository<int, Doctor>
    {
        protected DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Doctor> Get(int key)
        {
            var doctor = await _clinicContext.Doctors.SingleOrDefaultAsync(d => d.Id == key);
            return doctor ?? throw new KeyNotFoundException($"Doctor with ID {key} not found.");
        }

        public override async Task<IEnumerable<Doctor>> GetAll()
        {
            var doctors = await _clinicContext.Doctors.ToListAsync();
            if (doctors == null || doctors.Count() == 0)
                throw new KeyNotFoundException("No doctors in the database.");
            return doctors;
        }
    }
}