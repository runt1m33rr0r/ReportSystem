using ReportSystem.Data.Models;

namespace ReportSystem.Services.Contracts
{
    public interface IReportsService
    {
        void CreateReport(Report report);
    }
}