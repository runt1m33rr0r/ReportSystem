using System;
using System.Linq;
using System.Collections.Generic;
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

        public async Task<IActionResult> Reports(int? page)
        {
            IQueryable<ReportViewModel> reports = this.reportsService
                .GetAll()
                .Select(x => this.mapper.Map<ReportViewModel>(x));
            int pageSize = 5;
            var pagedReports = await PaginatedList<ReportViewModel>.CreateAsync(
                reports,
                page ?? 1,
                pageSize);
            ReportListViewModel viewModel = new ReportListViewModel()
            {
                Reports = pagedReports
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
        public async Task<IActionResult> SetReportStatus(ReportStatusViewModel reportStatus)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid report status!";

                return await this.Reports(reportStatus.Page);
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

                return await this.Reports(reportStatus.Page);
            }

            ViewData["Success"] = "Report updated!";

            return await this.Reports(reportStatus.Page);
        }
    }
}