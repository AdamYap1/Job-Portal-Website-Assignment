using Job_Portal_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Job_Portal_Website.Controllers
{
    public class ProfileController : Controller
    {
            private readonly ApplicationDbContext _context;
            private readonly string[] _allowedResumeTypes = { ".pdf", ".docx" };
            private const long MaxResumeSizeBytes = 5 * 1024 * 1024; // 5MB

            public ProfileController(ApplicationDbContext context)
            {
                _context = context;
            }

            private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // ---------- US-02: Edit Profile ----------

            [HttpGet]
            public IActionResult Edit()
            {
                var jobSeeker = _context.JobSeeker.Find(CurrentUserId);
                if (jobSeeker == null) return NotFound();
                return View(jobSeeker);
            }

            [HttpPost]
            public IActionResult Edit(string name, string skills, string experience, IFormFile resume)
            {
                var jobSeeker = _context.JobSeeker.Find(CurrentUserId);
                if (jobSeeker == null) return NotFound();

                jobSeeker.seekerName = name;
                jobSeeker.seekerSkills = skills;
                jobSeeker.seekerExp = experience;

                if (resume != null)
                {
                    var extension = Path.GetExtension(resume.FileName).ToLower();

                    if (!_allowedResumeTypes.Contains(extension))
                    {
                        ModelState.AddModelError("", "Resume must be a PDF or DOCX file.");
                        return View(jobSeeker);
                    }

                    if (resume.Length > MaxResumeSizeBytes)
                    {
                        ModelState.AddModelError("", "Resume file size must be under 5MB.");
                        return View(jobSeeker);
                    }

                    var fileName = $"{jobSeeker.jobSeekerId}_{Guid.NewGuid()}{extension}";
                    var savePath = Path.Combine("wwwroot/resumes", fileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        resume.CopyTo(stream);
                    }

                    jobSeeker.resumePath = $"/resumes/{fileName}";
                }

                _context.SaveChanges();
                return RedirectToAction("Edit");
            }
        }
    }