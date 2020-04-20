using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data.Models;

namespace ReportSystem.Data
{
    public class ReportSystemContext : IdentityDbContext
    {
        public ReportSystemContext(DbContextOptions<ReportSystemContext> options) : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }
    }
}