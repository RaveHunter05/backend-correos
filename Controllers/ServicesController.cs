using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class ServicesController : ControllerBase
	{
		private readonly CorreosContext _context;

		public ServicesController(CorreosContext context)
		{
			_context = context;
		}

		// GET: api/Services
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Service>>> GetServices()
		{
			if (_context.Services == null)
			{
				return NotFound();
			}
			return await _context.Services.OrderByDescending(service => service.Date).ToListAsync();
		}

		// GET: api/Services/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Service>> GetService(int id)
		{
			if (_context.Services == null)
			{
				return NotFound();
			}
			var service = await _context.Services.FindAsync(id);

			if (service == null)
			{
				return NotFound();
			}

			return service;
		}

		// GET: api/Services/search/name
		[HttpGet("search/{name}")]
		public async Task<IEnumerable<Service>> SearchByName(string name)
		{
			IQueryable<Service> query = _context.Services;

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(entity => entity.Name.Contains(name));
			}

			return await query.ToListAsync();
		}

		// PUT: api/Services/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutService(int id, Service service)
		{
			if (id != service.ServiceId)
			{
				return BadRequest();
			}

			_context.Entry(service).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ServiceExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Services
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Service>> PostService(Service service)
		{
			if (_context.Services == null)
			{
				return Problem("Entity set 'CorreosContext.Services'  is null.");
			}
			_context.Services.Add(service);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetService", new { id = service.ServiceId }, service);
		}

		// DELETE: api/Services/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
			if (_context.Services == null)
			{
				return NotFound();
			}
			var service = await _context.Services.FindAsync(id);
			if (service == null)
			{
				return NotFound();
			}

			_context.Services.Remove(service);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ServiceExists(int id)
		{
			return (_context.Services?.Any(e => e.ServiceId == id)).GetValueOrDefault();
		}
	}
}
