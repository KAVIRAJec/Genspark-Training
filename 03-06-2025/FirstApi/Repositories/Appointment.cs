using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class AppointmentRepository : Repository<string, Appointment>
    {
        public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Appointment> Get(string key)
        {
            var appointment = await _clinicContext.Appointments.
                                                    Include(a => a.Doctor).
                                                    Include(a => a.Patient).
                                                    SingleOrDefaultAsync(a => a.AppointmentNumber == key);
            return appointment ?? throw new KeyNotFoundException($"Appointment with ID {key} not found.");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = await _clinicContext.Appointments.
                                                    Include(a => a.Doctor).
                                                    Include(a => a.Patient).
                                                    ToListAsync();
            return appointments ?? new List<Appointment>();
        }
    }
}