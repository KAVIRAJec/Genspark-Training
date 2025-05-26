using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private static List<Patient> patients = new List<Patient>
    {
        new Patient { Id = 101, Name = "John Doe", Phone = "1234567890", DateOfBirth = new DateTime(1990, 1, 1) },
        new Patient { Id = 102, Name = "Jane Smith", Phone = "0987654321", DateOfBirth = new DateTime(1985, 5, 15) }
    };
    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetAllPatients()
    {
        if (patients == null || !patients.Any())
        {
            return NotFound("No patients found");
        }
        return Ok(patients);
    }

    [HttpPost]
    public ActionResult<Patient> AddPatient([FromBody] Patient patient)
    {
        if (patient == null)
        {
            return BadRequest("Invalid patient data");
        }
        patients.Add(patient);
        return CreatedAtAction(nameof(GetAllPatients), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public ActionResult<Patient> UpdatePatient(int id, [FromBody] Patient patient)
    {
        if (patient == null || patient.Id != id)
        {
            return BadRequest("Invalid patient data");
        }
        var existingPatient = patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            return NotFound("Patient not found");
        }
        existingPatient.Name = patient.Name;
        existingPatient.Phone = patient.Phone;
        existingPatient.DateOfBirth = patient.DateOfBirth;
        return Ok(existingPatient);
    }

    [HttpDelete("{id}")]
    public ActionResult<Patient> DeletePatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound("Patient not found");
        }
        patients.Remove(patient);
        return Ok(patient);
    }
}