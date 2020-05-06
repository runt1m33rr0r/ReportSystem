using ReportSystem.Utils;

namespace ReportSystem.Models
{
    public class ReportListViewModel
    {
        public PaginatedList<ReportViewModel> Reports { get; set; }
    }
}