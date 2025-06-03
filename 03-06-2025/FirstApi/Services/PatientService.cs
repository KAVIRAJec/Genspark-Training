using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Exceptions;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Services
{
    public class PatientService : IPatientService
    {
        PatientMapper patientMapper;
        private readonly IRepository<int, Patient> _patientRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IOtherContextFunctionalities _otherContextFunctionalities;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;
        private readonly IEncryptionService _encryptionService;

        public PatientService(IRepository<int, Patient> patientRepository,
                              IRepository<string, User> userRepository,
                              IOtherContextFunctionalities otherContextFunctionalities,
                              IMapper mapper,
                              IEncryptionService encryptionService,
                              ILogger<PatientService> logger)
        {
            patientMapper = new PatientMapper();
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _otherContextFunctionalities = otherContextFunctionalities;
            _mapper = mapper;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<Patient> AddPatient(PatientAddRequestDto patient)
        {
            try
            {
                var user = _mapper.Map<PatientAddRequestDto, User>(patient);
                var EncryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = patient.Password
                });
                user.Password = EncryptedData.EncryptedData;
                user.HashKey = EncryptedData.HashKey;
                user.Role = "Patient";
                user = await _userRepository.Add(user);

                var newPatient = patientMapper.MapPatientAddRequestPatient(patient);
                var existingPatients = await _patientRepository.GetAll();
                if (existingPatients.Any(p => p.Name.Equals(newPatient.Name, StringComparison.OrdinalIgnoreCase)))
                    throw new DuplicationEntryException("Patient with this name already exists");
                    
                newPatient = await _patientRepository.Add(newPatient);
                if (newPatient == null)
                    throw new Exception("Could not add patient");

                return newPatient;
            }
            catch (DuplicationEntryException ex)
            {
                _logger.LogError($"Duplication error: {ex.Message}");
                throw new DuplicationEntryException($"Duplication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding patient: {ex.Message}");
                throw new Exception($"Error adding patient: {ex.Message}");
            }
        }
        public async Task<Patient> GetPatientByName(string name)
        {
            try
            {
                var patients = await _patientRepository.GetAll();
                var patient = patients.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (patient == null)
                    throw new Exception("Patient not found");
                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving patient: {ex.Message}");
                throw new Exception($"Error retrieving patient: {ex.Message}");
            }
        }
    }
}