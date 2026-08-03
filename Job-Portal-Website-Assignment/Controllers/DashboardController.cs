using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{
    /// <summary>
    /// US-05 acceptance tests 1 and 6 require a successful login to land on the
    /// user's own dashboard, with a different destination per account type.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        // GET /Dashboard/JobSeeker
        [Authorize(Roles = "JobSeeker")]
        public IActionResult JobSeeker()
        {
            var seeker = _context.JobSeeker.Find(CurrentUserId);
            if (seeker == null) return RedirectToAction("Login", "Account");

            ViewBag.DisplayName = seeker.seekerName;
            ViewBag.HasResume = !string.IsNullOrWhiteSpace(seeker.resumePath);
            ViewBag.ApplicationCount = _context.Application.Count(a => a.jobSeekerId == CurrentUserId);

            return View();
        }

        // GET /Dashboard/Employer
        [Authorize(Roles = "Employer")]
        public IActionResult Employer()
        {
            var employer = _context.Employer.Find(CurrentUserId);
            if (employer == null) return RedirectToAction("Login", "Account");

            ViewBag.DisplayName = employer.companyName;
            ViewBag.ListingCount = _context.JobListing
                .Count(l => l.employerId == CurrentUserId && !l.isClosed && !l.isDeleted);

            return View();
        }
    }
}
