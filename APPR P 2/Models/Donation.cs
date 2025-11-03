using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPR_P_2.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DonorId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DonationType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; } // Changed to nullable

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        public List<string> Supplies { get; set; } = new List<string>();

        public string? AdditionalSupplies { get; set; }

        [Required]
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public bool IsAnonymous { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("DonorId")]
        public virtual ApplicationUser Donor { get; set; } = null!;
    }
}