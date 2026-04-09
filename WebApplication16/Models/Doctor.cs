<<<<<<< HEAD
п»їusing Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Doctor: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty; // РќР°РїСЂСЏРјРѕРє РјРµРґРёС†РёРЅРё
        public string Qualification { get; set; } = string.Empty; // РќР°РїСЂРёРєР»Р°Рґ, "MD", "PhD"
        public string LicenseNumber { get; set; } = string.Empty; // РњРµРґРёС‡РЅР° Р»С–С†РµРЅР·С–СЏ
=======
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
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
        public string OfficeAddress { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
<<<<<<< HEAD
        public ICollection<Patient> Patients { get; set; } // РџР°С†С–С”РЅС‚Рё Р»С–РєР°СЂСЏ
        public ICollection<Appointment> Appointments { get; set; } // РџСЂРёР№РѕРјРё
    }
}
=======
        public ICollection<Patient> Patients { get; set; } // Пацієнти лікаря
        public ICollection<Appointment> Appointments { get; set; } // Прийоми
    }
}
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
