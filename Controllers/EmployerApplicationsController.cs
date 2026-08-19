using Job_Portal_Website.Models;
using Job_Portal_Website.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployerApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int CurrentEmployerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Requirement 2: Retrieve all applications for a job listing owned by the logged-in employer
        [HttpGet]
        public async Task<IActionResult> ViewApplicants(int jobListId)
        {
            var jobListing = await _context.JobListing
                .FirstOrDefaultAsync(j => j.jobListId == jobListId);

            if (jobListing == null) return NotFound();

            // Ownership check — employer can only see applicants for their own listings
            if (jobListing.employerId != CurrentEmployerId)
                return Forbid();

            var applicants = await _context.Application
                .Where(a => a.jobListId == jobListId)
                .Include(a => a.JobSeeker)
                .OrderByDescending(a => a.appliedDate)
                .Select(a => new ApplicantRow
                {
                    applyId = a.applyId,
                    seekerName = a.JobSeeker.seekerName,
                    seekerEmail = a.JobSeeker.seekerEmail,
                    resumePath = a.JobSeeker.resumePath,
                    appliedDate = a.appliedDate,
                    applyStatus = a.applyStatus
                })
                .ToListAsync();

            var viewModel = new ApplicantListViewModel
            {
                jobListId = jobListId,
                jobTitle = jobListing.jobTitle,
                Applicants = applicants
            };

            return View(viewModel);
        }

        // Requirement 5 & 7: Update status to Accepted/Rejected, and allow changing a previous decision
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int applyId, int jobListId, ApplicationStatus newStatus)
        {
            var application = await _context.Application.FindAsync(applyId);
            if (application == null) return NotFound();

            var jobListing = await _context.JobListing.FindAsync(application.jobListId);
            if (jobListing == null || jobListing.employerId != CurrentEmployerId)
                return Forbid();

            // No restriction on current status — this allows switching Accepted <-> Rejected freely
            application.applyStatus = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewApplicants", new { jobListId });
        }
    }
}