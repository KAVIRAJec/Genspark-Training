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
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest("Name cannot be null or empty");
                var result = await _patientService.GetPatientByName(name);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient([FromBody] PatientAddRequestDto patient)
        {
            try
            {
                var newPatient = await _patientService.AddPatient(patient);
                if (newPatient != null)
                    return Created("", newPatient);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }        
    }
}