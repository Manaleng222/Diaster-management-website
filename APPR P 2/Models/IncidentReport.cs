using System;
using System.ComponentModel.DataAnnotations;

namespace APPR_P_2.Models
{
    public class IncidentReport
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Incident Type")]
        public required string IncidentType { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        public required string Severity { get; set; }

        [Required]
        [Display(Name = "Reported Date")]
        public DateTime ReportedDate { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty; // Default to empty string

        [Display(Name = "Status")]
        public string Status { get; set; } = "Reported";

        [Display(Name = "Report Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}