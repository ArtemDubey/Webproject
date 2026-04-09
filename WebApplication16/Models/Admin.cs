<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Admin: IdentityUser
=======
using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Admin : IdentityUser
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
    {
        public string? DoctorId { get; set; } = string.Empty;
        public Doctor? Doctor { get; set; }
        public string? PatientId { get; set; } = string.Empty;
        public Patient? Patient { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
<<<<<<< HEAD

=======
>>>>>>> f04bd7ee7d405866f2bcf1ccca7083b29349d490
    }
}
