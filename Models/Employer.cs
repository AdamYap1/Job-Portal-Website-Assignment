using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Website.Models
{
    public class Employer
    {
        [Key]
        public int employerId { get; set; }
        public string companyName { get; set; }
        public string employerEmail { get; set; }
        public string employerPassword { get; set; }
        public string? employerDescription { get; set; }
        public string? employerIndustry { get; set; }
        public string? logoPath { get; set; }

        public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
    }
}