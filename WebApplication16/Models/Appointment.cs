namespace WebApplication16.Models;

public class Appointment
{
    public int Id { get; set; }

    // Основні дані
    public DateTime Date { get; set; } // дата та час прийому
    public string? Reason { get; set; } // причина візиту (симптоми, консультація)

    // Зв’язки з іншими сутностями
    public string? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public string? PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int? PetId { get; set; }
    public Pet? Pet { get; set; }

    // Статус прийому
    public string Status { get; set; } = "Scheduled";
    // Наприклад: Scheduled, Completed, Cancelled

    // Метадані
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

}

