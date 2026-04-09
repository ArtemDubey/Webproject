using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;
using WebApplication16.Data;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TherapyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TherapyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Therapy>>> GetAll(
            [FromQuery] int? diseaseId,
            [FromQuery] string? search)
        {
            var query = _context.Therapies
                .Include(t => t.Disease)
                .AsQueryable();

            if (diseaseId.HasValue)
                query = query.Where(t => t.DiseaseId == diseaseId.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Name.Contains(search) || (t.Description != null && t.Description.Contains(search)));

            return Ok(await query.OrderBy(t => t.Name).ToListAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Therapy>> GetById(int id)
        {
            var therapy = await _context.Therapies
                .Include(t => t.Disease)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (therapy == null)
                return NotFound();

            return Ok(therapy);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<Therapy>> Create([FromBody] Therapy therapy)
        {
            var disease = await _context.Diseases.FindAsync(therapy.DiseaseId);
            if (disease == null)
                return BadRequest(new { message = "Хворобу не знайдено" });

            therapy.CreatedAt = DateTime.UtcNow;
            _context.Therapies.Add(therapy);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = therapy.Id }, therapy);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Update(int id, [FromBody] Therapy updated)
        {
            var therapy = await _context.Therapies.FindAsync(id);
            if (therapy == null)
                return NotFound();

            if (updated.DiseaseId != therapy.DiseaseId)
            {
                var diseaseExists = await _context.Diseases.AnyAsync(d => d.Id == updated.DiseaseId);
                if (!diseaseExists)
                    return BadRequest(new { message = "Хворобу не знайдено" });
            }

            therapy.Name = updated.Name;
            therapy.Description = updated.Description;
            therapy.DiseaseId = updated.DiseaseId;
            therapy.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(therapy);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var therapy = await _context.Therapies.FindAsync(id);
            if (therapy == null)
                return NotFound();

            _context.Therapies.Remove(therapy);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
