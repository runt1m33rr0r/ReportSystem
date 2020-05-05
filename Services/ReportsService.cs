using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data.Models;
using ReportSystem.Data.Repositories.Contracts;
using ReportSystem.Data.SaveContext.Contracts;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IEfRepository<Report> repository;
        private ISaveContext context;

        public ReportsService(IEfRepository<Report> repository, ISaveContext context)
        {
            this.repository = repository;
            this.context = context;
        }

        public void CreateReport(Report report)
        {
            this.repository.Add(report);
            this.context.Commit();
        }

        public IQueryable<Report> GetAll()
        {
            return this.repository.All.Include(r => r.Author);
        }

        public void SetReportStatus(int Id, ReportStatus status, string resolution)
        {
            Report report = this.repository.All.First(el => el.ID == Id);
            if (report == null)
            {
                throw new InvalidOperationException("Report does not exist!");
            }

            report.Status = status;
            report.Resolution = resolution;
            this.repository.Update(report);
            this.context.Commit();
        }
    }
}