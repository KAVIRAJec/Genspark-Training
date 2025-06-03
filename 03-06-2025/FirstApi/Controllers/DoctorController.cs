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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<DoctorsBySpecialityResponseDto>>> GetDoctors(string speciality)
        {
            try
            {
                if (string.IsNullOrEmpty(speciality))
                    return BadRequest("Speciality cannot be null or empty");
                var result = await _doctorService.GetDoctorsBySpeciality(speciality);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor([FromBody] DoctorAddRequestDto doctor)
        {
            try
            {
                var newDoctor = await _doctorService.AddDoctor(doctor);
                if (newDoctor != null)
                    return Created("", newDoctor);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}