using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class AppointmentRepository : Repository<int, Appointment>
    {
        protected AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Appointment> Get(int key)
        {
            var appointment = await _clinicContext.Appointments.SingleOrDefaultAsync(a => a.Id == key);
            return appointment ?? throw new KeyNotFoundException($"Appointment with ID {key} not found.");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = await _clinicContext.Appointments.ToListAsync();
            if (appointments == null || appointments.Count() == 0)
                throw new KeyNotFoundException("No appointments in the database.");
            return appointments;
        }
    }
}