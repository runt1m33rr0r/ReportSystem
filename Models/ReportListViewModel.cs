using ReportSystem.Utils;

namespace ReportSystem.Models
{
    public class ReportListViewModel
    {
        public PaginatedList<ReportViewModel> Reports { get; set; }
        public string Sort { get; set; }
        public string Search { get; set; }
        public string Status { get; set; }
    }
}