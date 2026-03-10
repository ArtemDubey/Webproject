using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Patient : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string? PetId { get; set; } // Власність для зберігання ідентифікатора зв'язаного Pet
        public Pet Pet { get; set; } // Власність для зв'язку з моделлю Pet
    }
}