using Microsoft.AspNetCore.Identity;

namespace WebApplication16.Models
{
    public class Admin : IdentityUser
    {
        public string? DoctorId { get; set; } = string.Empty;
        public Doctor? Doctor { get; set; }
        public string? PatientId { get; set; } = string.Empty;
        public Patient? Patient { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
