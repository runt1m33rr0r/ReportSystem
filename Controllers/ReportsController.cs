using Microsoft.AspNetCore.Mvc;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService reportsService;

        public ReportsController(IReportsService reportsService)
        {
            this.reportsService = reportsService;
        }

        public IActionResult Report()
        {
            return View();
        }
    }
}