using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
                string.IsNullOrWhiteSpace(listing.jobRequirements) ||
                string.IsNullOrWhiteSpace(listing.jobLocation) || 
                string.IsNullOrWhiteSpace(listing.employmentType))
            {
                ModelState.AddModelError("", "Fill in all required fields.");
                return View(listing);
            }

            if (listing.jobSalary <= 0)
            {
                ModelState.AddModelError("", "Salary must be a positive value");
                return View(listing);
            }

            listing.employerId = CurrentUserId;
            _context.JobListing.Add(listing);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = listing.jobListId });
        }

        // ================================================================
        // US-12: View the full details of a job listing
        // ================================================================
        //
        // Acceptance criteria coverage:
        //   AC-1 View active job listing ....... renders Details.cshtml
        //   AC-2 Invalid Job ID ................ "Job listing not found."
        //   AC-3 Closed job listing ............ "This job listing is no longer available."
        //   AC-4 Deleted job listing ........... "This job listing is no longer available."
        //   AC-5 Display company profile ....... Employer eager-loaded via Include
        //   AC-6 Role-based Apply button ....... ViewBag.CanApply
        //   AC-7 Back navigation ............... ViewBag.ReturnUrl
        //
        [HttpGet]
        public IActionResult Details(int id, string? returnUrl = null)
        {
            // Resolved first so the "unavailable" page also gets a working Back button.
            ViewBag.ReturnUrl = ResolveReturnUrl(returnUrl);

            var listing = _context.JobListing
                .Include(l => l.Employer)          // AC-5: company name + description
                .FirstOrDefault(l => l.jobListId == id);

            // AC-2: the JobID does not exist in the database.
            if (listing == null)
            {
                ViewBag.Message = "Job listing not found.";
                return View("Unavailable");
            }

            // AC-3 and AC-4: the listing exists but is Closed or soft-Deleted.
            if (listing.isClosed || listing.isDeleted)
            {
                ViewBag.Message = "This job listing is no longer available.";
                return View("Unavailable");
            }

            // AC-6: the Apply button is shown ONLY to a logged-in Job Seeker.
            // Employers and guests do not see it.
            ViewBag.CanApply = User.Identity != null
                               && User.Identity.IsAuthenticated
                               && User.IsInRole("JobSeeker");

            // AC-1
            return View(listing);
        }

        // ================================================================
        // US-18: Search jobs by Job Title or Company Name
        // ================================================================
        //
        // Acceptance criteria coverage:
        //   AC-1 Successful search ............. matches title OR company
        //   AC-2 No matching jobs .............. "No matching jobs found."
        //   AC-3 Partial keyword ............... Contains(), not equality
        //   AC-4 Case-insensitive .............. both sides lowered
        //   AC-5 Leading/trailing spaces ....... Trim()
        //   AC-6 Special character sanitisation  SanitiseKeyword()
        //   AC-7 Empty keyword ................. returns all Active jobs
        //   AC-8 Open Job Details .............. each row links to Details
        //
        [HttpGet]
        public IActionResult Search(string? keyword)
        {
            var raw = keyword ?? string.Empty;

            // AC-5: "  Engineer  " is treated as "Engineer".
            var trimmed = raw.Trim();

            // AC-6: "@Engineer#" is treated as "Engineer".
            var sanitised = SanitiseKeyword(trimmed);

            ViewBag.Keyword = raw;
            ViewBag.SanitisedKeyword = sanitised;

            // Only Active listings are searchable: not closed and not deleted.
            var activeJobs = _context.JobListing
                .Include(l => l.Employer)
                .Where(l => !l.isClosed && !l.isDeleted);

            List<JobListing> results;

            if (trimmed.Length == 0)
            {
                // AC-7: an empty keyword lists every active job.
                results = activeJobs
                    .OrderByDescending(l => l.postedDate)
                    .ToList();
            }
            else if (sanitised.Length == 0)
            {
                // The user typed something, but it was only special characters
                // (e.g. "@@@ ### !!!"). This is NOT the same as an empty search:
                // it must report no matches rather than list every job.
                results = new List<JobListing>();
            }
            else
            {
                // AC-3 partial match + AC-4 case-insensitive.
                // ToLower() on both sides translates to SQL LOWER(), so the
                // result does not depend on the database collation.
                var k = sanitised.ToLower();

                // AC-1: Job Title OR Company Name.
                results = activeJobs
                    .Where(l => l.jobTitle.ToLower().Contains(k)
                             || l.Employer.companyName.ToLower().Contains(k))
                    .OrderByDescending(l => l.postedDate)
                    .ToList();
            }

            // AC-2
            ViewBag.NoResults = results.Count == 0;

            return View(results);
        }

        /// <summary>
        /// Removes every character that is not a letter, a digit or whitespace,
        /// then collapses repeated whitespace. "@Engineer#" becomes "Engineer";
        /// "@@@ ### !!!" becomes an empty string.
        /// </summary>
        private static string SanitiseKeyword(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var cleaned = Regex.Replace(input, @"[^\p{L}\p{Nd}\s]", string.Empty);
            cleaned = Regex.Replace(cleaned, @"\s+", " ");
            return cleaned.Trim();
        }

        /// <summary>
        /// US-12 AC-7. Works out where the Back button should go: the explicit
        /// returnUrl supplied by the search results page, else the referring
        /// page if it belongs to this site, else the search page.
        /// </summary>
        private string ResolveReturnUrl(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer)
                && Uri.TryCreate(referer, UriKind.Absolute, out var uri)
                && string.Equals(uri.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase)
                && !uri.AbsolutePath.StartsWith("/Listings/Details", StringComparison.OrdinalIgnoreCase))
            {
                return uri.PathAndQuery;
            }

            return Url.Action(nameof(Search), "Listings") ?? "/Listings/Search";
        }
    }
}