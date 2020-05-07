using System.Linq;
using ReportSystem.Data.Models;

namespace ReportSystem.Services.Contracts
{
    public interface IReportsService
    {
        void CreateReport(Report report);
        IQueryable<Report> GetAll();
        IQueryable<Report> GetAll(string search, bool sortAscending, ReportStatus? status);
        void SetReportStatus(int Id, ReportStatus status, string resolution);
        void DeleteReport(int id);
    }
}