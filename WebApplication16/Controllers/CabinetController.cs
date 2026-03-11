using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Data;
using WebApplication16.Models;

namespace WebApplication16.Controllers
{
    [Authorize(Roles = "Patient")]
    public class CabinetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CabinetController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User) as Patient;
            if (user == null) return RedirectToAction("Login", "Account");

            var patient = await _context.Patients
                .Include(p => p.Pet)
                .FirstOrDefaultAsync(p => p.Id == user.Id);

            ViewBag.FirstName = patient?.FirstName;
            ViewBag.LastName = patient?.LastName;

            var pets = await _context.Pets
                .Where(p => p.PatientId == user.Id && p.IsActive)
                .ToListAsync();

            ViewBag.Pets = pets;
            ViewBag.PetsCount = pets.Count;

            var upcoming = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Pet)
                .Where(a => a.PatientId == user.Id && a.Date >= DateTime.Now && a.Status == "Scheduled")
                .OrderBy(a => a.Date)
                .Take(5)
                .ToListAsync();

            ViewBag.UpcomingAppointments = upcoming;
            ViewBag.UpcomingCount = upcoming.Count;

            ViewBag.TotalVisits = await _context.Appointments
                .CountAsync(a => a.PatientId == user.Id && a.Status == "Completed");

            return View();
        }

        public async Task<IActionResult> Appointments(string filter = "all")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.Filter = filter;

            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Pet)
                .Where(a => a.PatientId == user.Id)
                .AsQueryable();

            query = filter switch
            {
                "upcoming" => query.Where(a => a.Date >= DateTime.Now && a.Status == "Scheduled"),
                "completed" => query.Where(a => a.Status == "Completed"),
                "cancelled" => query.Where(a => a.Status == "Cancelled"),
                _ => query
            };

            ViewBag.Appointments = await query.OrderByDescending(a => a.Date).ToListAsync();
            return View();
        }

        public async Task<IActionResult> Pets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.Pets = await _context.Pets
                .Where(p => p.PatientId == user.Id && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> PetDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets
                .Include(p => p.Diseases)
                .Include(p => p.Therapies).ThenInclude(t => t.Disease)
                .FirstOrDefaultAsync(p => p.Id == id && p.PatientId == user.Id);

            if (pet == null) return NotFound();

            ViewBag.Pet = pet;
            ViewBag.Diseases = pet.Diseases;

            ViewBag.RecentVisits = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PetId == id)
                .OrderByDescending(a => a.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> MedicalCard(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets
                .Include(p => p.Diseases)
                .Include(p => p.Therapies).ThenInclude(t => t.Disease)
                .FirstOrDefaultAsync(p => p.Id == id && p.PatientId == user.Id);

            if (pet == null) return NotFound();

            ViewBag.Pet = pet;
            ViewBag.Diseases = pet.Diseases;
            ViewBag.Therapies = pet.Therapies;

            ViewBag.VisitHistory = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PetId == id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public IActionResult AddPet() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(string name, string species, string breed,
            string gender, DateTime? birthDate, string? allergies,
            string? chronicDiseases, string? vaccinations)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = new Pet
            {
                Name = name,
                Species = species,
                Breed = breed ?? string.Empty,
                Gender = gender,
                BirthDate = birthDate,
                Allergies = allergies,
                ChronicDiseases = chronicDiseases,
                Vaccinations = vaccinations,
                PatientId = user.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Pets");
        }

        [HttpGet]
        public async Task<IActionResult> EditPet(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.PatientId == user.Id);
            if (pet == null) return NotFound();

            ViewBag.Pet = pet;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(int id, string name, string breed,
            DateTime? birthDate, string? allergies, string? chronicDiseases, string? vaccinations)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.PatientId == user.Id);
            if (pet == null) return NotFound();

            pet.Name = name;
            pet.Breed = breed ?? string.Empty;
            pet.BirthDate = birthDate;
            pet.Allergies = allergies;
            pet.ChronicDiseases = chronicDiseases;
            pet.Vaccinations = vaccinations;
            pet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction("PetDetails", new { id });
        }
    }
}
