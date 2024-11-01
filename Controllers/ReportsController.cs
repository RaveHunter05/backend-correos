using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using correos_backend.Models.Enums;

using Microsoft.AspNetCore.Authorization;


namespace correos_backend.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ReportsController: ControllerBase
{
	private readonly CorreosContext _context;

	public ReportsController(CorreosContext context)
	{
		_context = context;
	}


	[HttpGet]
	public Task<IEnumerable<Report>> Get()
	{
		return Task.FromResult(_context.Reports.Where(r => r.Status != ApprovalStatus.Rejected).AsEnumerable());
	}

	// GET: api/ReportsController/5
	// To get a specific Report
	[HttpGet("{id}")]
	public async Task<ActionResult<Report>> Get(int id)
	{
		var report = await _context.Reports.Where(r => r.ReportId == id)
			.Where(r => r.Status != ApprovalStatus.Rejected)
			.FirstOrDefaultAsync();

		if (report == null)
		{
			return NotFound();
		}

		return report;
	}


	// Get: api/ReportsController/date
	// To get all Reports by date
	[HttpGet("{date}")]
	public Task<IEnumerable<Report>> Get(DateTime date)
	{
		return Task.FromResult(_context.Reports.Where(r => r.Date == date)
				.Where(r => r.Status != ApprovalStatus.Rejected)
				.AsEnumerable());
	}

	// POST: api/ReportsController
	// To create a new Report
	[HttpPost]
	public async Task<ActionResult> Create([FromForm] Report report)
	{
		if (report == null)
		{
			return BadRequest("Report is required");
		}

		if (string.IsNullOrEmpty(report.Title))
		{
			return BadRequest("Title is required");
		}

		if (string.IsNullOrEmpty(report.Description))
		{
			return BadRequest("Description is required");
		}

		if (report.AuthorId == 0)
		{
			return BadRequest("AuthorId is required");
		}


		var author = await _context.Users.FindAsync(report.AuthorId);

		if (author == null)
		{
			return BadRequest("Author not found");
		}

		if(report.FileName == null)
		{
			return BadRequest("File is required");
		}

		if(report.FileUrl == null)
		{
			return BadRequest("FileUrl is required");
		}

		_context.Reports.Add(report);
		await _context.SaveChangesAsync();

		return Ok();
	}

	// PUT: api/ReportsController/5 
	// Logical delete
	[HttpPut("{id}")]
	public async Task<ActionResult> Put(int id)
	{
		var report = await _context.Reports.FindAsync(id);

		if (report == null)
		{
			return NotFound();
		}

		report.Status = ApprovalStatus.Rejected;
		await _context.SaveChangesAsync();

		return Ok();
	}

}
