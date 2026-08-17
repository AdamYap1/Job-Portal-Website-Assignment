using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Website.Models
{
    public class JobListing
    {
        [Key]
        public int jobListId { get; set; }
        public string jobTitle { get; set; }
        public string jobDesc { get; set; }
        public string jobRequirements { get; set; }
        
        [Precision(18, 2)]
        public decimal jobSalary { get; set; }
        public string jobLocation { get; set; }

        // US-12: Employment Type must be shown on the Job Details page.
        public string employmentType { get; set; }

        // US-12: a listing can be Closed (employer stopped hiring) or Deleted
        // (soft delete). Both must show "This job listing is no longer
        // available.", and neither may appear in US-18 search results.
        public bool isClosed { get; set; } = false;
        public bool isDeleted { get; set; } = false;

        public DateTime postedDate { get; set; } = DateTime.Now;
        public int employerId { get; set; } 
        public Employer Employer { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}