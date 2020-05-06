using System.ComponentModel.DataAnnotations;
using ReportSystem.Data.Models;

namespace ReportSystem.Models
{
    public class ReportStatusViewModel
    {
        public int ID { get; set; }
        public ReportStatus Status { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 3, ErrorMessage = "The resolution must be at least {2} and at max {1} characters long.")]
        public string Resolution { get; set; }
        public byte[] Photo { get; set; }
        public int Page { get; set; }
    }
}