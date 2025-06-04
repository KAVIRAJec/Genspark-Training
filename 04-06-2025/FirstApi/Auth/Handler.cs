using System.Security.Claims;
using System.Threading.Tasks;
using FirstAPI.Models;
using FirstAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text.Json;

namespace FirstAPI.Auth
{
    public class ExperiencedDoctorHandler : AuthorizationHandler<ExperiencedDoctorRequirement>
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<string, Appointment> _appointmentRepository;

        public ExperiencedDoctorHandler(IRepository<int, Doctor> doctorRepository,
            IRepository<string, Appointment> appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ExperiencedDoctorRequirement requirement)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value 
                ?? context.User.FindFirst("role")?.Value;

            if (role != "Doctor")
                return;

            var email = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("nameid")?.Value;

            if (string.IsNullOrEmpty(email))
                return;

            string appointmentNumber = null;
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                var key = mvcContext.RouteData.Values.Keys
                .FirstOrDefault(k => string.Equals(k, "appointmentNumber", StringComparison.OrdinalIgnoreCase));
                if (key != null)
                {
                    appointmentNumber = mvcContext.RouteData.Values[key]?.ToString();
                }
            }
            else if (context.Resource is HttpContext httpContext)
            {
                appointmentNumber = httpContext.Request.RouteValues
                    .FirstOrDefault(kv => string.Equals(kv.Key, "appointmentNumber", StringComparison.OrdinalIgnoreCase)).Value?.ToString();
            }
            else if (context.Resource is Stream stream)
            {
                return;
            }

            if (string.IsNullOrEmpty(appointmentNumber))
            {
                Console.WriteLine("Appointment number not found in route.");
                return;
            }

            var appointment = await _appointmentRepository.Get(appointmentNumber);
            if (appointment == null)
                return;

            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d =>
                d.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
                return;

            if (appointment.DoctorId == doctor.Id && doctor.YearsOfExperience >= requirement.MinimumYears)
            {
                context.Succeed(requirement);
                Console.WriteLine($"Doctor: {doctor.Email}, Experience: {doctor.YearsOfExperience}, Required: {requirement.MinimumYears}");
                Console.WriteLine($"Appointment.DoctorId: {appointment.DoctorId}, Doctor.Id: {doctor.Id}");
            }
        }
    }
}
