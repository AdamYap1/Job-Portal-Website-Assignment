using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // ---------- US-08: Post Job Listing (Employer only) ----------

        [Authorize(Roles = "Employer")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Roles = "Employer")]
        [HttpPost]
        public IActionResult Create(JobListing listing)
        {
            if (string.IsNullOrWhiteSpace(listing.jobTitle) ||
                string.IsNullOrWhiteSpace(listing.jobDesc) ||
                string.IsNullOrWhiteSpace(listing.jobRequirements))
            {
                ModelState.AddModelError("", "Title, description, and requirements are required.");
                return View(listing);
            }

            listing.employerId = CurrentUserId;
            _context.JobListing.Add(listing);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = listing.jobListId });
        }

        // ---------- US-12: View Listing Details ----------

        [HttpGet]
        public IActionResult Details(int id)
        {
            var listing = _context.JobListing
                .Include(l => l.Employer)
                .FirstOrDefault(l => l.jobListId == id);

            if (listing == null || listing.isClosed)
            {
                return NotFound("This job listing is no longer available.");
            }

            return View(listing);
        }

        // ---------- US-18: Search by Text ----------

        [HttpGet]
        public IActionResult Search(string keyword)
        {
            var query = _context.JobListing.Where(l => !l.isClosed);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(l => l.jobTitle.Contains(keyword));
            }

            var results = query.Include(l => l.Employer).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.NoResults = !results.Any();

            return View(results);
        }
    }
}