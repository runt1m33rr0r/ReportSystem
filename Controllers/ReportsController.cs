using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReportSystem.Data.Models;
using ReportSystem.Models;
using ReportSystem.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ReportSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService reportsService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ReportsController(
            IReportsService reportsService,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.reportsService = reportsService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public IActionResult Report()
        {
            return View();
        }

        public IActionResult Reports()
        {
            ICollection<ReportViewModel> reports = this.reportsService
                .GetAll()
                .Select(x => this.mapper.Map<ReportViewModel>(x))
                .ToList();

            ReportListViewModel viewModel = new ReportListViewModel()
            {
                Reports = reports
            };

            return View("Reports", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Report(ReportViewModel report)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid report!";

                return View();
            }

            Report reportEntity = this.mapper.Map<Report>(report);
            reportEntity.CreationDate = DateTime.Now;
            reportEntity.Status = ReportStatus.OPEN;
            reportEntity.Author = await this.userManager.GetUserAsync(User);
            this.reportsService.CreateReport(reportEntity);

            ViewData["Success"] = "Report created!";

            return View();
        }

        [HttpPost]
        public IActionResult SetReportStatus(ReportStatusViewModel reportStatus)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid report status!";

                return this.Reports();
            }

            try
            {
                this.reportsService.SetReportStatus(
                    reportStatus.ID,
                    reportStatus.Status,
                    reportStatus.Resolution);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["Error"] = ex.Message;

                return this.Reports();
            }

            ViewData["Success"] = "Report updated!";

            return this.Reports();
        }
    }
}