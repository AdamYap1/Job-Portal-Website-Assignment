using Job_Portal_Website.Helpers;
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

        // ================================================================
        // US-01: Job Seeker Registration
        // ================================================================
        //
        // Acceptance tests:
        //   1 Successful registration ....... success message + redirect to Login
        //   2 Duplicate email address ....... "Email address is already registered."
        //   3 Email format validation ....... "Please enter a valid email address."
        //   4 Password constraint ........... PasswordRequirementMessage
        //   5 Password confirmation ......... "Passwords do not match."
        //   6 Required fields cannot be empty per-field "... is required."
        //
        [HttpGet]
        public IActionResult RegisterJobSeeker() => View();

        [HttpPost]
        public IActionResult RegisterJobSeeker(string name, string email, string password, string confirmPassword)
        {
            ValidateRegistration("Name", name, email, password, confirmPassword);

            if (!ModelState.IsValid)
            {
                return View();
            }

            var jobSeeker = new JobSeeker
            {
                seekerName = name.Trim(),
                seekerEmail = ValidationHelper.NormaliseEmail(email),
                seekerPassword = _hasher.HashPassword(null!, password),
                isActive = true
            };

            _context.JobSeeker.Add(jobSeeker);
            _context.SaveChanges();

            // Test 1: a success message must be displayed after redirecting.
            // TempData survives exactly one redirect, which is what we need here.
            TempData["SuccessMessage"] = "Registration successful. Please log in with your new account.";
            return RedirectToAction("Login");
        }

        // ================================================================
        // US-03: Employer Registration
        // ================================================================
        // Same six acceptance tests as US-01. The Sprint 1 review found this
        // form had no password validation at all; it now shares exactly the
        // same rules as the job seeker form via ValidateRegistration.
        //
        [HttpGet]
        public IActionResult RegisterEmployer() => View();

        [HttpPost]
        public IActionResult RegisterEmployer(string companyName, string email, string password, string confirmPassword)
        {
            ValidateRegistration("Company name", companyName, email, password, confirmPassword);

            if (!ModelState.IsValid)
            {
                return View();
            }

            var employer = new Employer
            {
                companyName = companyName.Trim(),
                employerEmail = ValidationHelper.NormaliseEmail(email),
                employerPassword = _hasher.HashPassword(null!, password),
                isActive = true
            };

            _context.Employer.Add(employer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. Please log in with your new account.";
            return RedirectToAction("Login");
        }

        /// <summary>
        /// All registration validation for US-01 and US-03 in one place.
        /// Errors are added per field so the form can point at what went wrong.
        /// </summary>
        private void ValidateRegistration(string nameLabel, string nameValue,
                                          string email, string password, string confirmPassword)
        {
            // Test 6: required fields.
            if (string.IsNullOrWhiteSpace(nameValue))
            {
                ModelState.AddModelError("name", $"{nameLabel} is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Email address is required.");
            }
            else if (!ValidationHelper.IsValidEmail(email))
            {
                // Test 3
                ModelState.AddModelError("email", ValidationHelper.EmailFormatMessage);
            }
            else if (EmailAlreadyRegistered(email))
            {
                // Test 2
                ModelState.AddModelError("email", "Email address is already registered.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }
            else if (!ValidationHelper.IsValidPassword(password))
            {
                // Test 4
                ModelState.AddModelError("password", ValidationHelper.PasswordRequirementMessage);
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("confirmPassword", "Please confirm your password.");
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                // Test 5
                ModelState.AddModelError("confirmPassword", "Passwords do not match.");
            }
        }

        /// <summary>
        /// An email may only be used once across the WHOLE site, not once per
        /// table. Login searches job seekers first, so allowing the same address
        /// in both tables would make the employer account permanently
        /// unreachable.
        /// </summary>
        private bool EmailAlreadyRegistered(string email)
        {
            var normalised = ValidationHelper.NormaliseEmail(email);

            return _context.JobSeeker.Any(js => js.seekerEmail == normalised)
                || _context.Employer.Any(e => e.employerEmail == normalised);
        }

        // ================================================================
        // US-05: Login
        // ================================================================
        //
        // Acceptance tests:
        //   1 Successful login .............. redirect to the account dashboard
        //   2 Required login fields ......... per-field "... is required."
        //   3 Invalid email format .......... "Please enter a valid email address."
        //   4 Invalid login credentials ..... "Invalid email or password."
        //   5 Inactive account .............. "Your account is inactive."
        //   6 Redirect by account type ...... JobSeeker vs Employer dashboard
        //
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Test 2: blank fields get their own message rather than falling
            // through to the generic "Invalid email or password."
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Email address is required.");
            }
            else if (!ValidationHelper.IsValidEmail(email))
            {
                // Test 3
                ModelState.AddModelError("email", ValidationHelper.EmailFormatMessage);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var normalisedEmail = ValidationHelper.NormaliseEmail(email);

            // Try job seeker first.
            var jobSeeker = _context.JobSeeker.FirstOrDefault(js => js.seekerEmail == normalisedEmail);
            if (jobSeeker != null &&
                _hasher.VerifyHashedPassword(null!, jobSeeker.seekerPassword, password) == PasswordVerificationResult.Success)
            {
                // Test 5. Checked AFTER the password is verified on purpose: if we
                // checked first, an attacker could learn which emails exist simply
                // by watching for the "inactive" message.
                if (!jobSeeker.isActive)
                {
                    ModelState.AddModelError("", "Your account is inactive.");
                    return View();
                }

                await SignInUser(jobSeeker.jobSeekerId.ToString(), jobSeeker.seekerEmail, "JobSeeker");

                // Tests 1 and 6
                return RedirectToAction("JobSeeker", "Dashboard");
            }

            // Then employer.
            var employer = _context.Employer.FirstOrDefault(e => e.employerEmail == normalisedEmail);
            if (employer != null &&
                _hasher.VerifyHashedPassword(null!, employer.employerPassword, password) == PasswordVerificationResult.Success)
            {
                if (!employer.isActive)
                {
                    ModelState.AddModelError("", "Your account is inactive.");
                    return View();
                }

                await SignInUser(employer.employerId.ToString(), employer.employerEmail, "Employer");

                // Tests 1 and 6
                return RedirectToAction("Employer", "Dashboard");
            }

            // Test 4. One generic message covers a wrong password, a wrong email
            // and an account that does not exist, so none of those can be told
            // apart from outside.
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

        // ================================================================
        // US-06: Logout
        // ================================================================
        // Acceptance test: the session ends and the user returns to the Login page.
        //
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }
}
