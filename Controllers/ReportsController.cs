using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReportSystem.Data.Models;
using ReportSystem.Models;
using ReportSystem.Services.Contracts;
using ReportSystem.Utils;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

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

        private bool IsFileExtensionValid(IFormFile file)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }

        private bool IsPhotoSizeValid(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);

                return memoryStream.Length < 5242880;
            }
        }

        public IActionResult Report()
        {
            return View();
        }

        public async Task<IActionResult> Reports(
            int? page,
            string search,
            string sort,
            string status,
            bool personal = false)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("User"))
            {
                personal = false;
            }

            ReportStatus? statusFilter = null;
            if (!String.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "open":
                        statusFilter = ReportStatus.OPEN;
                        break;
                    case "in_progress":
                        statusFilter = ReportStatus.IN_PROGRESS;
                        break;
                    case "resolved":
                        statusFilter = ReportStatus.RESOLVED;
                        break;
                    default:
                        statusFilter = ReportStatus.REFUSED;
                        break;
                }
            }

            IQueryable<ReportViewModel> reports = this.reportsService.GetAll(
                search,
                sort == "newest_first" || sort == null,
                statusFilter)
                .Where(rep =>
                    personal ?
                    this.User.Identity.IsAuthenticated && rep.Author.UserName == this.User.Identity.Name :
                    true)
                .Select(x => this.mapper.Map<ReportViewModel>(x));

            int pageSize = 5;
            int chosenPage = page ?? 1;
            var pagedReports = await PaginatedList<ReportViewModel>.CreateAsync(
                reports,
                chosenPage,
                pageSize);
            if (pagedReports.Count == 0 && page > 0)
            {
                chosenPage = 1;
                pagedReports = await PaginatedList<ReportViewModel>.CreateAsync(
                    reports,
                    chosenPage,
                    pageSize);
            }

            ReportListViewModel viewModel = new ReportListViewModel()
            {
                Reports = pagedReports,
                Search = !String.IsNullOrEmpty(search) ? search : "",
                Sort = !String.IsNullOrEmpty(sort) ? sort : "newest_first",
                Status = status,
                Personal = personal
            };

            return View("Reports", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Report(ReportViewModel report)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid report!";

                return View();
            }

            Report reportEntity = this.mapper.Map<Report>(report);
            if (report.PhotoFile != null)
            {
                if (!this.IsFileExtensionValid(report.PhotoFile))
                {
                    ViewData["Error"] = "Invalid photo file extension!";

                    return View();
                }

                if (!this.IsPhotoSizeValid(report.PhotoFile))
                {
                    ViewData["Error"] = "Photo size is too large! Maximum photo size is 5MB!";

                    return View();
                }

                using (var memoryStream = new MemoryStream())
                {
                    report.PhotoFile.CopyTo(memoryStream);
                    reportEntity.Photo = memoryStream.ToArray();
                }
            }

            reportEntity.CreationDate = DateTime.Now;
            reportEntity.Status = ReportStatus.OPEN;
            reportEntity.Author = await this.userManager.GetUserAsync(User);
            this.reportsService.CreateReport(reportEntity);

            ViewData["Success"] = "Report created!";

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> SetReportStatus(ReportStatusViewModel reportStatus)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid report status!";

                return await this.Reports(
                    reportStatus.Page,
                    reportStatus.Search,
                    reportStatus.Order,
                    reportStatus.StatusFilter);
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

                return await this.Reports(
                    reportStatus.Page,
                    reportStatus.Search,
                    reportStatus.Order,
                    reportStatus.StatusFilter);
            }

            ViewData["Success"] = "Report updated!";

            return await this.Reports(
                reportStatus.Page,
                reportStatus.Search,
                reportStatus.Order,
                reportStatus.StatusFilter);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteReport(DeleteReportViewModel deleteReportViewModel)
        {
            try
            {
                this.reportsService.DeleteReport(deleteReportViewModel.ID);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["Error"] = ex.Message;

                return await this.Reports(
                    deleteReportViewModel.Page,
                    deleteReportViewModel.Search,
                    deleteReportViewModel.Order,
                    deleteReportViewModel.StatusFilter,
                    deleteReportViewModel.ShowPersonalReports);
            }

            ViewData["Success"] = "Report deleted!";

            return await this.Reports(
                deleteReportViewModel.Page,
                deleteReportViewModel.Search,
                deleteReportViewModel.Order,
                deleteReportViewModel.StatusFilter,
                deleteReportViewModel.ShowPersonalReports);
        }
    }
}