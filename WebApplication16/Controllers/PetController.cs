using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetAll(
            [FromQuery] string? species,
            [FromQuery] string? patientId,
            [FromQuery] string? search)
        {
            var query = _context.Pets
                .Include(p => p.Patients)
                .Include(p => p.Diseases)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(species))
                query = query.Where(p => p.Species == species);
            if (!string.IsNullOrEmpty(patientId))
                query = query.Where(p => p.PatientId == patientId);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Breed.Contains(search));

            return Ok(await query.OrderBy(p => p.Name).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetById(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Patients)
                .Include(p => p.Diseases)
                .Include(p => p.Therapies).ThenInclude(t => t.Disease)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound();

            return Ok(pet);
        }

        [HttpGet("owner/{patientId}")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetByOwner(string patientId)
        {
            return Ok(await _context.Pets
                .Include(p => p.Diseases)
                .Where(p => p.PatientId == patientId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Pet>> Create([FromBody] Pet pet)
        {
            if (!string.IsNullOrEmpty(pet.PatientId))
            {
                var ownerExists = await _context.Patients.AnyAsync(p => p.Id == pet.PatientId);
                if (!ownerExists)
                    return BadRequest(new { message = "Власника не знайдено" });
            }

            pet.IsActive = true;
            pet.CreatedAt = DateTime.UtcNow;
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        [HttpPost("{id}/diseases/{diseaseId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> AddDisease(int id, int diseaseId)
        {
            var pet = await _context.Pets
                .Include(p => p.Diseases)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null)
                return NotFound(new { message = "Тварину не знайдено" });

            var disease = await _context.Diseases.FindAsync(diseaseId);
            if (disease == null)
                return NotFound(new { message = "Хворобу не знайдено" });

            if (pet.Diseases.Any(d => d.Id == diseaseId))
                return Conflict(new { message = "Хвороба вже прив'язана" });

            pet.Diseases.Add(disease);
            pet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/diseases/{diseaseId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> RemoveDisease(int id, int diseaseId)
        {
            var pet = await _context.Pets
                .Include(p => p.Diseases)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null)
                return NotFound();

            var disease = pet.Diseases.FirstOrDefault(d => d.Id == diseaseId);
            if (disease == null)
                return NotFound(new { message = "Хвороба не прив'язана до цієї тварини" });

            pet.Diseases.Remove(disease);
            pet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Pet updated)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            pet.Name = updated.Name;
            pet.Breed = updated.Breed;
            pet.BirthDate = updated.BirthDate;
            pet.Vaccinations = updated.Vaccinations;
            pet.Allergies = updated.Allergies;
            pet.ChronicDiseases = updated.ChronicDiseases;
            pet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(pet);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            pet.IsActive = false;
            pet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
