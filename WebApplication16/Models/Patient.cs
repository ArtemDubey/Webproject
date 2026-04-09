<<<<<<< HEAD
ï»¿using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Patient: IdentityUser
=======
using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Patient : IdentityUser
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;

<<<<<<< HEAD
        public string? PetId { get; set; } // Ð’Ð»Ð°ÑÐ½Ñ–ÑÑ‚ÑŒ Ð´Ð»Ñ Ð·Ð±ÐµÑ€Ñ–Ð³Ð°Ð½Ð½Ñ Ñ–Ð´ÐµÐ½Ñ‚Ð¸Ñ„Ñ–ÐºÐ°Ñ‚Ð¾Ñ€Ð° Ð·Ð²'ÑÐ·Ð°Ð½Ð¾Ð³Ð¾ Pet
        public Pet Pet { get; set; } // Ð’Ð»Ð°ÑÐ½Ñ–ÑÑ‚ÑŒ Ð´Ð»Ñ Ð·Ð²'ÑÐ·ÐºÑƒ Ð· Ð¼Ð¾Ð´ÐµÐ»Ð»ÑŽ Pet
    }
}
=======
        public string? PetId { get; set; } // Âëàñí³ñòü äëÿ çáåð³ãàííÿ ³äåíòèô³êàòîðà çâ'ÿçàíîãî Pet
        public Pet Pet { get; set; } // Âëàñí³ñòü äëÿ çâ'ÿçêó ç ìîäåëëþ Pet
    }
}
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
