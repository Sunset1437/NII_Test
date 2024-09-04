using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NII_Test.DataBase;
using NII_Test.MDL;
using System.Numerics;

namespace NII_Test.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DoctorsController(ApplicationDbContext dbContext) 
        {
            _context = dbContext;
        }
        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctors(string sortBy = "LastName", int page = 1, int pageSize = 10)
        {
            var doctors = _context.Doctors.Include(d => d.Specialization)
                .Select(d => new DoctorListDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    SpecializationName = d.Specialization.Name
                });

            // Sorting
            switch (sortBy.ToLower())
            {
                case "firstname":
                    doctors = doctors.OrderBy(d => d.FirstName);
                    break;
                case "lastname":
                    doctors = doctors.OrderBy(d => d.LastName);
                    break;
                case "specialization":
                    doctors = doctors.OrderBy(d => d.SpecializationName);
                    break;
                default:
                    doctors = doctors.OrderBy(d => d.LastName);
                    break;
            }

            // Pagination
            var pagedDoctors = await doctors.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(pagedDoctors);
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorEditDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var doctorEditDto = new DoctorEditDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                SpecializationId = doctor.SpecializationId
            };

            return Ok(doctorEditDto);
        }

        // POST: api/Doctors
        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor(DoctorEditDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = new Doctor
            {
                FirstName = doctorDto.FirstName,
                LastName = doctorDto.LastName,
                SpecializationId = doctorDto.SpecializationId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }
        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDoctor(int id, DoctorEditDto doctorDto)
        {
            if (id != doctorDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctorInDb = await _context.Doctors.FindAsync(id);
            if (doctorInDb == null)
            {
                return NotFound();
            }

            doctorInDb.FirstName = doctorDto.FirstName;
            doctorInDb.LastName = doctorDto.LastName;
            doctorInDb.SpecializationId = doctorDto.SpecializationId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctorInDb = await _context.Doctors.FindAsync(id);
            if (doctorInDb == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctorInDb);
            await _context.SaveChangesAsync();

            return Ok(doctorInDb);
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
