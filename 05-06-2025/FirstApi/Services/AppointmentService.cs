using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<string, Appointment> _appointmentRepository;
        private readonly IRepository<int, Patient> _patientRepository;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly AppointmentMapper _appointmentMapper;
        public AppointmentService(IRepository<string, Appointment> appointmentRepository,
                                 IRepository<int, Patient> patientRepository,
                                 IRepository<int, Doctor> doctorRepository)
        {
            _appointmentMapper = new AppointmentMapper();
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment)
        {
            try
            {
                if (appointment == null)
                    throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null");
                var newAppointment = _appointmentMapper.MapAppointmentAddRequestDto(appointment);
                if (newAppointment == null)
                    throw new Exception("Could not map appointment data");
                var appointments = await _appointmentRepository.GetAll();

                if (appointments == null || appointments.Count() == 0)
                {
                    await _appointmentRepository.Add(newAppointment);
                    return newAppointment;                    
                }
                if (appointments.Any(a => a.AppointmentNumber == newAppointment.AppointmentNumber) || appointments.Any(a => a.AppointmentDateTime == newAppointment.AppointmentDateTime && a.DoctorId == newAppointment.DoctorId))
                    throw new Exception("Appointment already exists for the given time with the same doctor");
                await _appointmentRepository.Add(newAppointment);
                return newAppointment;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Appointment> GetAppointmentByNumber(string appointmentNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(appointmentNumber))
                    throw new ArgumentNullException(nameof(appointmentNumber), "Appointment number cannot be null or empty");
                return await _appointmentRepository.Get(appointmentNumber);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int patientId)
        {
            try
            {
                if (patientId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(patientId), "Patient ID must be greater than zero");
                var appointments = await _appointmentRepository.GetAll();
                List<Appointment> patientAppointments = new();
                foreach (var appointment in appointments)
                {
                    if (appointment.PatientId == patientId)
                        patientAppointments.Add(appointment);
                }
                return patientAppointments ?? throw new Exception("No appointments found for the given patient ID");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(int doctorId)
        {
            try
            {
                if (doctorId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(doctorId), "Doctor ID must be greater than zero");
                var appointments = await _appointmentRepository.GetAll();
                List<Appointment> doctorAppointments = new();
                foreach (var appointment in appointments)
                {
                    if (appointment.DoctorId == doctorId)
                        doctorAppointments.Add(appointment);
                }
                return doctorAppointments ?? throw new Exception("No appointments found for the given doctor ID");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }   
        }
        public async Task<Appointment> UpdateAppointment(string appointmentNumber, AppointmentAddRequestDto appointment)
        {
            try
            {
                if (appointment == null)
                    throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null");
                var existingAppointment = await _appointmentRepository.Get(appointmentNumber);
                if (existingAppointment == null)
                    throw new Exception("Appointment not found");
                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.DoctorId = appointment.DoctorId;
                existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
                await _appointmentRepository.Update(existingAppointment.AppointmentNumber, existingAppointment);
                return existingAppointment;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Appointment> DeleteAppointment(string appointmentNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(appointmentNumber))
                    throw new ArgumentNullException(nameof(appointmentNumber), "Appointment number cannot be null or empty");
                var appointment = await _appointmentRepository.Get(appointmentNumber);
                if (appointment == null)
                    throw new Exception("Appointment not found");
                // await _appointmentRepository.Delete(appointmentNumber);
                appointment.Status = "Cancelled";
                await _appointmentRepository.Update(appointment.AppointmentNumber, appointment);
                return appointment;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}