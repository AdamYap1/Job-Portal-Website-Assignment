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
        public IActionResult Apply(int jobListId)
        {
            var jobSeeker = _context.JobSeeker.Find(CurrentUserId);
            var listing = _context.JobListing.Find(jobListId);

            if (listing == null || listing.isClosed || listing.isDeleted)
            {
                TempData["ApplyError"] = "This job listing is no longer available.";
                return RedirectToAction("Details", "Listings", new { id = jobListId });
            }

            if (jobSeeker == null)
            {
                return BadRequest("User not found.");
            }

            if (string.IsNullOrEmpty(jobSeeker.resumePath))
            {
                TempData["ApplyError"] = "Please upload a resume to your profile before applying.";
                TempData["ShowResumeLink"] = true;
                return RedirectToAction("Details", "Listings", new { id = jobListId });
            }

            bool alreadyApplied = _context.Application
                .Any(a => a.jobSeekerId == CurrentUserId && a.jobListId == jobListId);

            if (alreadyApplied)
            {
                TempData["ApplyError"] = "You have already applied to this job listing.";
                return RedirectToAction("Details", "Listings", new { id = jobListId });
            }

            var application = new Application
            {
                jobSeekerId = CurrentUserId,
                jobListId = jobListId,
                applyStatus = ApplicationStatus.Submitted
            };

            _context.Application.Add(application);
            _context.SaveChanges();

            TempData["ApplySuccess"] = "Your application has been submitted successfully.";
            // Stay on the Details page instead of redirecting to MyApplications
            return RedirectToAction("Details", "Listings", new { id = jobListId });
        }

        // ---------- Cancel Application ----------

        [HttpPost]
        public IActionResult Cancel(int applyId, string returnTo = "details")
        {
            var application = _context.Application.Find(applyId);

            if (application == null || application.jobSeekerId != CurrentUserId)
            {
                return NotFound();
            }

            int jobListId = application.jobListId;

            _context.Application.Remove(application);
            _context.SaveChanges();

            if (returnTo == "myapplications")
            {
                TempData["ApplySuccess"] = "Application withdrawn.";
                return RedirectToAction("MyApplications");
            }

            TempData["ApplySuccess"] = "Application withdrawn.";
            return RedirectToAction("Details", "Listings", new { id = jobListId });
        }

        [HttpGet]
        public IActionResult MyApplications()
        {
            var jobSeeker = _context.JobSeeker.Find(CurrentUserId);
            ViewBag.ResumePath = jobSeeker?.resumePath;
            var applications = _context.Application
                .Where(a => a.jobSeekerId == CurrentUserId)
                .Select(a => new
                {
                    a.applyId,
                    a.jobListId,
                    a.JobListing.jobTitle,
                    a.applyStatus,
                    a.appliedDate
                })
                .OrderByDescending(a => a.appliedDate)
                .ToList();

            return View(applications);
        }
    }
}