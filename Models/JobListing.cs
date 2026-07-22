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
        
        [Precision(5, 2)]
        public decimal jobSalary { get; set; }
        public string jobLocation { get; set; }
        public bool isClosed { get; set; } = false;
        public DateTime postedDate { get; set; } = DateTime.Now;
        public int employerId { get; set; } 
        public Employer Employer { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}