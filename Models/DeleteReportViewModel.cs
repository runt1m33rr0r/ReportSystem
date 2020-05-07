namespace ReportSystem.Models
{
    public class DeleteReportViewModel
    {
        public int ID { get; set; }
        public int? Page { get; set; }
        public string Search { get; set; }
        public string Order { get; set; }
        public string StatusFilter { get; set; }
    }
}