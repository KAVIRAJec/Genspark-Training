using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class PatientRepository : Repository<int, Patient>
    {
        protected PatientRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Patient> Get(int key)
        {
            var patient = await _clinicContext.Patients.SingleOrDefaultAsync(p => p.Id == key);
            return patient ?? throw new KeyNotFoundException($"Patient with ID {key} not found.");
        }

        public override async Task<IEnumerable<Patient>> GetAll()
        {
            var patients = await _clinicContext.Patients.ToListAsync();
            if (patients == null || patients.Count() == 0)
                throw new KeyNotFoundException("No patients in the database.");
            return patients;
        }
    }
}