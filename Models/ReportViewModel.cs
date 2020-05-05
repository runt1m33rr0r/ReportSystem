using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ReportSystem.Data.Models;

namespace ReportSystem.Models
{
    public class ReportViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 3, ErrorMessage = "The title must be at least {2} and at max {1} characters long.")]
        public string Title { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 3, ErrorMessage = "The description must be at least {2} and at max {1} characters long.")]
        public string Description { get; set; }
        public string Resolution { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public IdentityUser Author { get; set; }
    }
}