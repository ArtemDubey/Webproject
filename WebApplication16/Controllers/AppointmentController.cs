using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAll(
            [FromQuery] DateTime? date,
            [FromQuery] string? doctorId,
            [FromQuery] string? patientId,
            [FromQuery] string? status)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Pet)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(a => a.Date.Date == date.Value.Date);
            if (!string.IsNullOrEmpty(doctorId))
                query = query.Where(a => a.DoctorId == doctorId);
            if (!string.IsNullOrEmpty(patientId))
                query = query.Where(a => a.PatientId == patientId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return Ok(await query.OrderBy(a => a.Date).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetById(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpGet("doctor/{doctorId}/schedule")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetDoctorSchedule(
            string doctorId,
            [FromQuery] DateTime? date)
        {
            var targetDate = date?.Date ?? DateTime.Today;

            return Ok(await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Pet)
                .Where(a => a.DoctorId == doctorId && a.Date.Date == targetDate)
                .OrderBy(a => a.Date)
                .ToListAsync());
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPatientAppointments(string patientId)
        {
            return Ok(await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Pet)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Create([FromBody] Appointment appointment)
        {
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId && d.IsActive);
            if (!doctorExists)
                return BadRequest(new { message = "Лікаря не знайдено або він неактивний" });

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.Date.Date == appointment.Date.Date &&
                a.Date.Hour == appointment.Date.Hour &&
                a.Status != "Cancelled");

            if (conflict)
                return Conflict(new { message = "Лікар вже має запис у цей час" });

            appointment.Status = "Scheduled";
            appointment.CreatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Appointment updated)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.Date = updated.Date;
            appointment.Reason = updated.Reason;
            appointment.DoctorId = updated.DoctorId;
            appointment.PatientId = updated.PatientId;
            appointment.PetId = updated.PetId;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> PatchStatus(int id, [FromBody] string status)
        {
            var validStatuses = new[] { "Scheduled", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
                return BadRequest(new { message = "Невірний статус" });

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
