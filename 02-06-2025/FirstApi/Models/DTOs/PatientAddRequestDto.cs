namespace FirstAPI.Models.DTOs.DoctorSpecialities
{
    public class PatientAddRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}