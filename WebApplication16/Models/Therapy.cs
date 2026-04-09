<<<<<<< HEAD
п»їnamespace WebApplication16.Models
{
    public class Therapy
    {
    }
}
=======
namespace WebApplication16.Models
{
    public class Therapy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Назва терапії
        public string? Description { get; set; }        // Опис терапії
        public int DiseaseId { get; set; }             // Зовнішній ключ до хвороби
        public Disease Disease { get; set; }           // Навігаційна властивість до хвороби
        // Метадані
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
