using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Auth
{
    public class ExperiencedDoctorRequirement : IAuthorizationRequirement
    {
        public int MinimumYears { get; }
        public ExperiencedDoctorRequirement(int minimumYears)
        {
            MinimumYears = minimumYears;
        }
    }
}