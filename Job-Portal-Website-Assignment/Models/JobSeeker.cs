using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Website.Models
{
    public class JobSeeker
    {
        [Key]
        public int jobSeekerId { get; set; }
        public string seekerName { get; set; }
        public string seekerEmail { get; set; }
        public string seekerPassword { get; set; }
        public string?seekerSkills { get; set; }
        public string? seekerExp {  get; set; }
        public string? resumePath { get; set; }

        // US-05 "Inactive account" acceptance test. New accounts are active so
        // that a user can log in immediately after registering; an administrator
        // (or the tester) sets this to false to exercise the inactive path.
        public bool isActive { get; set; } = true;

        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}