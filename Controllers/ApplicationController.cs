using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // ---------- US-13: Apply with Saved Resume ----------

        [HttpPost]
        public IActionResult Apply(int jobListingId)
        {
            var jobSeeker = _context.JobSeeker.Find(CurrentUserId);
            var listing = _context.JobListing.Find(jobListingId);

            if (listing == null || listing.isClosed)
            {
                return BadRequest("This job listing is no longer available.");
            }

            if (string.IsNullOrEmpty(jobSeeker.resumePath))
            {
                return BadRequest("Please upload a resume to your profile before applying.");
            }

            bool alreadyApplied = _context.Application
                .Any(a => a.jobSeekerId == CurrentUserId && a.jobListId == jobListingId);

            if (alreadyApplied)
            {
                return BadRequest("You have already applied to this job listing.");
            }

            var application = new Application
            {
                jobSeekerId = CurrentUserId,
                jobListId = jobListingId,
                applyStatus = ApplicationStatus.Submitted
            };

            _context.Application.Add(application);
            _context.SaveChanges();

            return RedirectToAction("MyApplications");
        }

        [HttpGet]
        public IActionResult MyApplications()
        {
            var applications = _context.Application
                .Where(a => a.jobSeekerId == CurrentUserId)
                .Select(a => new { a.applyId, a.JobListing.jobTitle, a.applyStatus, a.appliedDate })
                .ToList();

            return View(applications);
        }
    }
}