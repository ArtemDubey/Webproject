namespace WebApplication16.Models
{
    public class Pet
    {
        public int Id { get; set; }

        // Основні дані
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty; // Наприклад: Dog, Cat
        public string Breed { get; set; } = string.Empty; // Порода
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Медична інформація
        public string? Vaccinations { get; set; }
        public string? Allergies { get; set; }
        public string? ChronicDiseases { get; set; }

        // Зв’язки з іншими сутностями
        public string? PatientId { get; set; }
        public Patient? Patients { get; set; } // Власник тварини
        public ICollection<Disease> Diseases { get; set; }
        public ICollection<Therapy> Therapies { get; set; }

        // Метадані
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

    }
}