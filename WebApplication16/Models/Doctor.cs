using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Doctor : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty; // Напрямок медицини
        public string Qualification { get; set; } = string.Empty; // Наприклад, "MD", "PhD"
        public string LicenseNumber { get; set; } = string.Empty; // Медична ліцензія
        public string OfficeAddress { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Patient> Patients { get; set; } // Пацієнти лікаря
        public ICollection<Appointment> Appointments { get; set; } // Прийоми
    }
}