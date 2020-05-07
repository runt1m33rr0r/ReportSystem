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

        public IQueryable<Report> GetAll(string search, bool sortAscending, ReportStatus? status)
        {
            var reports = this.GetAll();
            if (sortAscending)
            {
                reports = reports.OrderBy(rep => rep.CreationDate);
            }
            else
            {
                reports = reports.OrderByDescending(rep => rep.CreationDate);
            }

            if (!String.IsNullOrEmpty(search) && !String.IsNullOrWhiteSpace(search))
            {
                reports = reports.Where(rep =>
                    rep.Title.Contains(search) ||
                    rep.Description.Contains(search) ||
                    rep.Resolution.Contains(search));
            }

            if (status != null)
            {
                reports = reports.Where(rep => rep.Status == status);
            }

            return reports;
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