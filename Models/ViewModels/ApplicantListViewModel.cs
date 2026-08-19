using Job_Portal_Website.Models;

namespace Job_Portal_Website.Models.ViewModels
{
    public class ApplicantListViewModel
    {
        public int jobListId { get; set; }
        public string jobTitle { get; set; }
        public List<ApplicantRow> Applicants { get; set; } = new List<ApplicantRow>();
    }

    public class ApplicantRow
    {
        public int applyId { get; set; }
        public string seekerName { get; set; }
        public string seekerEmail { get; set; }
        public string resumePath { get; set; }
        public DateTime appliedDate { get; set; }
        public ApplicationStatus applyStatus { get; set; }
    }
}