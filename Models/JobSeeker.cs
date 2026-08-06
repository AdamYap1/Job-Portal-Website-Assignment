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
        public string? seekerSkills { get; set; }
        public string? seekerExp { get; set; }
        public string? resumePath { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}