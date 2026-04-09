using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Data;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAll()
        {
            return Ok(await _context.Admins
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.IsActive)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetById(string id)
        {
            var admin = await _context.Admins
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null)
                return NotFound();

            return Ok(admin);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboard()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            return Ok(new
            {
                Totals = new
                {
                    ActiveDoctors = await _context.Doctors.CountAsync(d => d.IsActive),
                    TotalPatients = await _context.Patients.CountAsync(),
                    TotalPets = await _context.Pets.CountAsync(p => p.IsActive),
                    TotalAppointments = await _context.Appointments.CountAsync(),
                    TotalDiseases = await _context.Diseases.CountAsync(),
                    TotalTherapies = await _context.Therapies.CountAsync()
                },
                Today = new
                {
                    Total = await _context.Appointments.CountAsync(a => a.Date.Date == today),
                    Scheduled = await _context.Appointments.CountAsync(a => a.Date.Date == today && a.Status == "Scheduled"),
                    Completed = await _context.Appointments.CountAsync(a => a.Date.Date == today && a.Status == "Completed"),
                    Cancelled = await _context.Appointments.CountAsync(a => a.Date.Date == today && a.Status == "Cancelled")
                },
                ThisMonth = new
                {
                    Appointments = await _context.Appointments.CountAsync(a => a.Date >= thisMonth)
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Admin updated)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            if (!string.IsNullOrEmpty(updated.DoctorId))
            {
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == updated.DoctorId);
                if (!doctorExists)
                    return BadRequest(new { message = "Лікаря не знайдено" });
                admin.DoctorId = updated.DoctorId;
            }

            if (!string.IsNullOrEmpty(updated.PatientId))
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == updated.PatientId);
                if (!patientExists)
                    return BadRequest(new { message = "Пацієнта не знайдено" });
                admin.PatientId = updated.PatientId;
            }

            admin.PhoneNumber = updated.PhoneNumber;
            admin.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(admin);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(string id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            admin.IsActive = false;
            admin.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
