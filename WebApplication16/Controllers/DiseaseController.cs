using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;
using WebApplication16.Data;

namespace WebApplication16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiseaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiseaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Disease>>> GetAll(
            [FromQuery] string? category,
            [FromQuery] bool? isInfectious,
            [FromQuery] string? search)
        {
            var query = _context.Diseases
                .Include(d => d.Therapies)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(d => d.Category == category);
            if (isInfectious.HasValue)
                query = query.Where(d => d.IsInfectious == isInfectious.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.Name.Contains(search) || (d.Description != null && d.Description.Contains(search)));

            return Ok(await query.OrderBy(d => d.Name).ToListAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Disease>> GetById(int id)
        {
            var disease = await _context.Diseases
                .Include(d => d.Therapies)
                .Include(d => d.Pets)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (disease == null)
                return NotFound();

            return Ok(disease);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<Disease>> Create([FromBody] Disease disease)
        {
            var duplicate = await _context.Diseases.AnyAsync(d => d.Name == disease.Name);
            if (duplicate)
                return Conflict(new { message = $"Хвороба «{disease.Name}» вже існує" });

            disease.CreatedAt = DateTime.UtcNow;
            _context.Diseases.Add(disease);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = disease.Id }, disease);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> Update(int id, [FromBody] Disease updated)
        {
            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
                return NotFound();

            disease.Name = updated.Name;
            disease.Description = updated.Description;
            disease.Category = updated.Category;
            disease.IsInfectious = updated.IsInfectious;
            disease.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(disease);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var disease = await _context.Diseases
                .Include(d => d.Therapies)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (disease == null)
                return NotFound();

            if (disease.Therapies.Any())
                return BadRequest(new { message = "Неможливо видалити: є пов'язані терапії" });

            _context.Diseases.Remove(disease);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
