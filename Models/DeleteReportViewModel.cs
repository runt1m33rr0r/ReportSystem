namespace ReportSystem.Models
{
    public class DeleteReportViewModel
    {
        public int ID { get; set; }
        public int? Page { get; set; }
        public string Search { get; set; }
        public string Order { get; set; }
        public string StatusFilter { get; set; }
        public bool ShowPersonalReports { get; set; } = false;

        public DeleteReportViewModel() { }

        public DeleteReportViewModel(
            int id,
            int? page,
            string search,
            string order,
            string statusFilter,
            bool showPersonal)
        {
            this.ID = id;
            this.Page = page;
            this.Search = search;
            this.Order = order;
            this.StatusFilter = statusFilter;
            this.ShowPersonalReports = showPersonal;
        }
    }
}