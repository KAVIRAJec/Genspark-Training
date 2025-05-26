using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class DoctorController : ControllerBase
{
    static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor{Id=101,Name="Ramu"},
        new Doctor{Id=102,Name="Somu"},
    };
    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetDoctors()
    {
        return Ok(doctors);
    }
    [HttpPost]
    public ActionResult<Doctor> CreateDoctor([FromBody] Doctor doctor)
    {
        doctors.Add(doctor);
        return Created("", doctor);
    }

    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdateDoctor(int id, [FromBody] Doctor doctor)
    {
        if (doctor == null || doctor.Id != id)
        {
            return BadRequest("Invalid doctor data");
        }
        var existingDoctor = doctors.FirstOrDefault(d => d.Id == id);
        if (existingDoctor == null)
        {
            return NotFound("Doctor not found");
        }
        existingDoctor.Name = doctor.Name;
        return Ok(existingDoctor);
    }

    [HttpDelete("{id}")]
    public ActionResult<Doctor> DeleteDoctor(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound("Doctor not found");
        }
        doctors.Remove(doctor);
        return Ok(doctor);
    }
}