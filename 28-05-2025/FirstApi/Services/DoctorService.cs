using FirstAPI.Interfaces;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository
        }

        public async Task<Doctor> GetDoctorByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("Doctor name cannot be null or empty");
            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (doctor == null) throw new Exception($"Doctor with name {name} not found");
            if (doctor.Status != "Active") throw new Exception($"Doctor {name} is not active");
            return doctor;
        }
        public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality)
        {
            if (string.IsNullOrEmpty(speciality)) throw new Exception("Speciality cannot be null or empty");
            var speciality = await _specialityRepository.GetAll();
            var matchedSpeciality = specialities.FirstOrDefault(s => s.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));
            if (matchedSpeciality == null)
                throw new Exception($"Speciality '{speciality}' not found.");

            var matchedDoctorIds = doctorSpecialities.Where(ds => ds.SpecialityId == matchedSpeciality.Id)
                .Select(ds => ds.DoctorId).Distinct().ToList();

            if (!matchedDoctorIds.Any())
                throw new Exception($"No doctors found for speciality '{speciality}'.");

            var doctors = await _doctorRepository.GetAll();
            var filteredDoctors = doctors.Where(d => matchedDoctorIds.Contains(d.Id)).ToList();

            return filteredDoctors ?? throw new Exception($"Doctor with {speciality} speciality not found");
        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
        {
            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                YearsOfExperience = doctorDto.YearsOfExperience,
                Status = "Active"
            };
 
            var addedDoctor = await _doctorRepository.Add(doctor);
 
            if (doctorDto.Specialities != null)
            {
                foreach (var specDto in doctorDto.Specialities)
                {
                    var speciality = (await _specialityRepository.GetAll()).FirstOrDefault(s => s.Name.ToLower() == specDto.Name.ToLower());
 
                    if (speciality == null)
                    {
                        speciality = await _specialityRepository.Add(new Speciality
                        {
                            Name = specDto.Name,
                            Status = "Active"
                        });
                    }

                    if (speciality.Status != "Active") throw new Exception($"Speciality {specDto.Name} is not active");

                    await _doctorSpecialityRepository.Add(new DoctorSpeciality
                    {
                        DoctorId = addedDoctor.Id,
                        SpecialityId = speciality.Id
                    });
                }
            }
            return addedDoctor;
        }
    }
}