using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;
using WebApplication16.Data;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetAll(
            [FromQuery] string? specialty,
            [FromQuery] string? clinicName)
        {
            var query = _context.Doctors
                .Where(d => d.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(specialty))
                query = query.Where(d => d.Specialty.Contains(specialty));
            if (!string.IsNullOrEmpty(clinicName))
                query = query.Where(d => d.ClinicName == clinicName);

            return Ok(await query.OrderBy(d => d.LastName).ToListAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Doctor>> GetById(string id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Update(string id, [FromBody] Doctor updated)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.FirstName = updated.FirstName;
            doctor.LastName = updated.LastName;
            doctor.Specialty = updated.Specialty;
            doctor.Qualification = updated.Qualification;
            doctor.LicenseNumber = updated.LicenseNumber;
            doctor.OfficeAddress = updated.OfficeAddress;
            doctor.ClinicName = updated.ClinicName;
            doctor.PhoneNumber = updated.PhoneNumber;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(doctor);
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ToggleActive(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.IsActive = !doctor.IsActive;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(doctor);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            doctor.IsActive = false;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
