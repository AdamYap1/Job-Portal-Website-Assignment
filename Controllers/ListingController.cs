using System;

public class ListingController : Controller
{
    private readonly ApplicationDbContext _context;

    public ListingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // US-08: Post job listing
    [HttpPost]
    public IActionResult Create(JobListing listing)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.JobListings.Add(listing);
        _context.SaveChanges();
        return Ok(listing);
    }

    // US-12: View listing details
    [HttpGet]
    public IActionResult Details(int id)
    {
        var listing = _context.JobListings
            .Include(l => l.Employer)
            .FirstOrDefault(l => l.Id == id);

        if (listing == null || listing.IsClosed)
            return NotFound("Listing not available");

        return Ok(listing);
    }

    // US-18: Search by text
    [HttpGet]
    public IActionResult Search(string keyword)
    {
        var results = _context.JobListings
            .Where(l => !l.IsClosed && l.Title.Contains(keyword))
            .ToList();

        return Ok(results);
    }
}
