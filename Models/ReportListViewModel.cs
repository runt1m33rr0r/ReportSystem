using System.Collections.Generic;

namespace ReportSystem.Models
{
    public class ReportListViewModel
    {
        public ICollection<ReportViewModel> Reports { get; set; }
    }
}