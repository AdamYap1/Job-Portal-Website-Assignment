using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{ 
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<object> _hasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        //US-01:Job Seeker Registration

        [HttpGet]
        public IActionResult RegisterJobSeeker() => View();

        [HttpPost]
        public IActionResult RegisterJobSeeker(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError("", "Password must be at least 8 characters.");
                return View();
            }

            if (_context.JobSeeker.Any(js => js.seekerEmail == email))
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View();
            }

            var jobSeeker = new JobSeeker
            {
                seekerName = name,
                seekerEmail = email,
                seekerPassword = _hasher.HashPassword(null, password)
            };

            _context.JobSeeker.Add(jobSeeker);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        //US-03:Employer Registration

        [HttpGet]
        public IActionResult RegisterEmployer() => View();

        [HttpPost]
        public IActionResult RegisterEmployer(string companyName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (_context.Employer.Any(e => e.employerEmail == email))
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View();
            }

            var employer = new Employer
            {
                companyName = companyName,
                employerEmail = email,
                employerPassword = _hasher.HashPassword(null, password)
            };

            _context.Employer.Add(employer);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        //US-05:Login

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Try job seeker first
            var jobSeeker = _context.JobSeeker.FirstOrDefault(js => js.seekerEmail == email);
            if (jobSeeker != null &&
                _hasher.VerifyHashedPassword(null, jobSeeker.seekerPassword, password) == PasswordVerificationResult.Success)
            {
                await SignInUser(jobSeeker.jobSeekerId.ToString(), jobSeeker.seekerEmail, "JobSeeker");
                return RedirectToAction("Index", "Home");
            }

            // Try employer
            var employer = _context.Employer.FirstOrDefault(e => e.employerEmail == email);
            if (employer != null &&
                _hasher.VerifyHashedPassword(null, employer.employerPassword, password) == PasswordVerificationResult.Success)
            {
                await SignInUser(employer.employerId.ToString(), employer.employerEmail, "Employer");
                return RedirectToAction("Index", "Home");
            }

            // Generic error - don't reveal whether email or password was wrong
            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        private async Task SignInUser(string userId, string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        //US-06:Logout

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        //US-14: Access Denied

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
