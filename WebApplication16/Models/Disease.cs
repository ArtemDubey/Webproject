
namespace WebApplication16.Models;

public class Disease
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Назва хвороби
    public string? Description { get; set; }        // Опис або симптоми
    public string? Category { get; set; }       // Категорія (інфекційна, хронічна тощо)
    public bool IsInfectious { get; set; } = false; // Чи є хвороба інфекційною

    // Метадані
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Зв’язки з іншими сутностями
    public ICollection<Pet> Pets { get; set; }
    public ICollection<Therapy> Therapies { get; set; }

}
