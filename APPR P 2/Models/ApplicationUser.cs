using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace APPR_P_2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string? UserRole { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public virtual VolunteerProfile? VolunteerProfile { get; set; }
        public virtual ICollection<IncidentReport> IncidentReports { get; set; } = new List<IncidentReport>();
    }
}