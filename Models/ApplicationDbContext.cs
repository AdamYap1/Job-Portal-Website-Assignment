using Microsoft.EntityFrameworkCore;
namespace Job_Portal_Website.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<JobSeeker> JobSeeker { get; set; }
        public DbSet<Employer> Employer { get; set; }
        public DbSet<JobListing> JobListing { get; set; }
        public DbSet<Application> Application { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Duplicate checks
            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.jobSeekerId, a.jobListId })
                .IsUnique();

            modelBuilder.Entity<JobSeeker>()
                .HasIndex(j => j.seekerEmail)
                .IsUnique();

            modelBuilder.Entity<Employer>()
                .HasIndex(e => e.employerEmail)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}