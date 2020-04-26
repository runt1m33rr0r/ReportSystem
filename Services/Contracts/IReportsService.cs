using System.Linq;
using ReportSystem.Data.Models;

namespace ReportSystem.Services.Contracts
{
    public interface IReportsService
    {
        void CreateReport(Report report);
        IQueryable<Report> GetAll();
        void SetReportStatus(int Id, ReportStatus status, string resolution);
    }
}