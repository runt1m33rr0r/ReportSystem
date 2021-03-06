using System;
using Microsoft.AspNetCore.Identity;

namespace ReportSystem.Data.Models
{
    public enum ReportStatus
    {
        OPEN, IN_PROGRESS, RESOLVED, REFUSED
    }

    public class Report : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Resolution { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public IdentityUser Author { get; set; }
        public byte[] Photo { get; set; }
    }
}