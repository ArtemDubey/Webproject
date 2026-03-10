using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Patients
                .Include(p => p.Pet)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    (p.Email != null && p.Email.Contains(search)) ||
                    (p.PhoneNumber != null && p.PhoneNumber.Contains(search)));

            var total = await query.CountAsync();
            var patients = await query
                .OrderBy(p => p.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, page, pageSize, data = patients });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetById(string id)
        {
            var patient = await _context.Patients
                .Include(p => p.Pet)
                    .ThenInclude(pet => pet != null ? pet.Diseases : null)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpGet("{id}/appointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(string id)
        {
            return Ok(await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Pet)
                .Where(a => a.PatientId == id)
                .OrderByDescending(a => a.Date)
                .ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Patient updated)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            patient.FirstName = updated.FirstName;
            patient.LastName = updated.LastName;
            patient.BirthDate = updated.BirthDate;
            patient.Gender = updated.Gender;
            patient.PhoneNumber = updated.PhoneNumber;

            await _context.SaveChangesAsync();
            return Ok(patient);
        }
    }
}
