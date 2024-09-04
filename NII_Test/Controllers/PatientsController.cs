using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NII_Test.DataBase;
using NII_Test.MDL;
using System.Net;

namespace NII_Test.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PatientsController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients(string sortBy = "LastName", int page = 1, int pageSize = 10)
        {
            var patients = _context.Patients.Include(p => p.DoctorId)
                .Select(p => new PatientListDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    //DoctorName = p.Doctor.FirstName + " " + p.Doctor.LastName
                });

            // Sorting
            switch (sortBy.ToLower())
            {
                case "firstname":
                    patients = patients.OrderBy(p => p.FirstName);
                    break;
                case "lastname":
                    patients = patients.OrderBy(p => p.LastName);
                    break;
                case "dateofbirth":
                    patients = patients.OrderBy(p => p.DateOfBirth);
                    break;
                default:
                    patients = patients.OrderBy(p => p.LastName);
                    break;
            }

            // Pagination
            var pagedPatients = await patients.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(pagedPatients);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientEditDto>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var patientEditDto = new PatientEditDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                DoctorId = patient.DoctorId
            };

            return Ok(patientEditDto);
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(PatientEditDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                DoctorId = patientDto.DoctorId
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPatient(int id, PatientEditDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patientInDb = await _context.Patients.FindAsync(id);
            if (patientInDb == null)
            {
                return NotFound();
            }

            patientInDb.FirstName = patientDto.FirstName;
            patientInDb.LastName = patientDto.LastName;
            patientInDb.DateOfBirth = patientDto.DateOfBirth;
            patientInDb.DoctorId = patientDto.DoctorId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patientInDb = await _context.Patients.FindAsync(id);
            if (patientInDb == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patientInDb);
            await _context.SaveChangesAsync();

            return Ok(patientInDb);
        }
    }
}
