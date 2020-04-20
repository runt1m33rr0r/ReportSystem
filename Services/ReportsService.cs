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
    }
}