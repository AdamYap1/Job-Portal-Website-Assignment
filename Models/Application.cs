using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Website.Models
{
    public enum ApplicationStatus
    {
        Submitted,
        Viewed,
        Accepted,
        Rejected
    }

    public class Application
    {
        [Key]
        public int applyId {  get; set; }
        public int jobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
        public int jobListId { get; set; }
        public JobListing JobListing { get; set; }
        public ApplicationStatus applyStatus { get; set; } = ApplicationStatus.Submitted;
        public DateTime appliedDate { get; set; } = DateTime.Now;

    }
}