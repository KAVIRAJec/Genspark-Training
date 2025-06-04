using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Route("AddAppointment")]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentAddRequestDto appointment)
        {
            try
            {
                if (appointment == null)
                    return BadRequest("Appointment cannot be null");
                
                var newAppointment = await _appointmentService.AddAppointment(appointment);
                if (newAppointment != null)
                    return Created("", newAppointment);
                
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAppointment/{appointmentNumber}")]
        [Authorize(Policy = "ExperiencedDoctorOnly")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] string appointmentNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(appointmentNumber))
                    return BadRequest("Appointment number cannot be null or empty");

                var response = await _appointmentService.DeleteAppointment(appointmentNumber);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAppointmentById/{appointmentNumber}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] string appointmentNumber)
        {
            if (string.IsNullOrEmpty(appointmentNumber))
                return BadRequest("Appointment number cannot be null or empty");

            var response = await _appointmentService.GetAppointmentByNumber(appointmentNumber);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetAppointmentsByPatientId/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatientId([FromRoute] int patientId)
        {
            try
            {
                var response = await _appointmentService.GetAppointmentsByPatientId(patientId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAppointmentsByDoctorId/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctorId([FromRoute] int doctorId)
        {
            try
            {
                var response = await _appointmentService.GetAppointmentsByDoctorId(doctorId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("UpdateAppointment/{appointmentNumber}")]
        public async Task<IActionResult> UpdateAppointment([FromRoute] string appointmentNumber, [FromBody] AppointmentAddRequestDto appointment)
        {
            try
            {
                if (appointment == null || string.IsNullOrEmpty(appointmentNumber))
                    return BadRequest("Appointment data or appointment number cannot be null or empty");

                var updatedAppointment = await _appointmentService.UpdateAppointment(appointmentNumber, appointment);
                return Ok(updatedAppointment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}