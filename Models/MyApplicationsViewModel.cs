using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Website.Models
{
    public class MyApplicationsViewModel
    {
        public int ApplyId { get; set; }
        public int JobListId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public ApplicationStatus ApplyStatus { get; set; }
        public DateTime AppliedDate { get; set; }

        /// <summary>
        /// Returns the status as displayed to the user.
        /// Maps Submitted to Pending for display purposes.
        /// </summary>
        public string DisplayStatus
        {
            get
            {
                return ApplyStatus switch
                {
                    ApplicationStatus.Submitted => "Pending",
                    ApplicationStatus.Viewed => "Viewed",
                    ApplicationStatus.Accepted => "Accepted",
                    ApplicationStatus.Rejected => "Rejected",
                    _ => ApplyStatus.ToString()
                };
            }
        }
    }
}
