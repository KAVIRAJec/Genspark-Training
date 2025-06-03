using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        DoctorMapper doctorMapper;
        SpecialityMapper specialityMapper;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IOtherContextFunctionalities _otherContextFunctionalities;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;
        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
                            IRepository<string, User> userRepository,
                            IOtherContextFunctionalities otherContextFunctionalities,
                            IMapper mapper,
                            IEncryptionService encryptionService)
        {
            doctorMapper = new DoctorMapper();
            specialityMapper = new SpecialityMapper();
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
            _userRepository = userRepository;
            _otherContextFunctionalities = otherContextFunctionalities;
            _mapper = mapper;
            _encryptionService = encryptionService;
        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
        {
            try
            {
                var user = _mapper.Map<DoctorAddRequestDto, User>(doctor);
                var EncryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = doctor.Password
                });
                user.Password = EncryptedData.EncryptedData;
                user.HashKey = EncryptedData.HashKey;
                user.Role = "Doctor";
                user = await _userRepository.Add(user);

                var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
                newDoctor = await _doctorRepository.Add(newDoctor);
                if (newDoctor == null)
                    throw new Exception("Could not add doctor");
                if (doctor.Specialities.Count > 0)
                {
                    int[] specialities = await MapAndAddSpeciality(doctor);
                    for (int i = 0; i < specialities.Length; i++)
                    {
                        var doctorSpeciality = specialityMapper.MapDoctorSpeciality(newDoctor.Id, specialities[i]);
                        doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                    }
                }
                return newDoctor;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<int[]> MapAndAddSpeciality(DoctorAddRequestDto doctor)
        {
            int[] specialityIds = new int[doctor.Specialities.Count()];
            IEnumerable<Speciality> existingSpecialities = null;
            try
            {
                existingSpecialities = await _specialityRepository.GetAll();
            }
            catch (Exception e)
            {

            }
            int count = 0;
            foreach (var item in doctor.Specialities)
            {
                Speciality speciality = null;
                if (existingSpecialities != null)
                    speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
                if (speciality == null)
                {
                    speciality = specialityMapper.MapSpecialityAddRequestDoctor(item);
                    speciality = await _specialityRepository.Add(speciality);
                }
                specialityIds[count] = speciality.Id;
                count++;
            }
            return specialityIds;
        }
        

        public async Task<Doctor> GetDoctorByName(string name)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            var result = await _otherContextFunctionalities.GetDoctorsBySpeciality(speciality);
            return result;
        }
    }
}