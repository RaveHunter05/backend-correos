using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using correos_backend.Services;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class CostCentersController : ControllerBase
	{
		private readonly CorreosContext _context;
		private readonly CsvService _csvService;
		private readonly CurrentTimeService _currentTimeService;

		public CostCentersController(CorreosContext context, CsvService csvService, CurrentTimeService currentTimeService)
		{
			_context = context;
			_csvService = csvService;
			_currentTimeService = currentTimeService;
		}

		// GET: api/CostCenters
		[HttpGet]
		public async Task<ActionResult<IEnumerable<CostCenter>>> GetCostCenters()
		{
			if (_context.CostCenters == null)
			{
				return NotFound();
			}
			return await _context.CostCenters.OrderByDescending(costCenter => costCenter.Date).ToListAsync();
		}

		// GET: api/CostCenters/5
		[HttpGet("{id}")]
		public async Task<ActionResult<CostCenter>> GetCostCenter(int id)
		{
			if (_context.CostCenters == null)
			{
				return NotFound();
			}
			var costCenter = await _context.CostCenters.FindAsync(id);

			if (costCenter == null)
			{
				return NotFound();
			}

			return costCenter;
		}

		// GET: api/CostCenters/search/name
		[HttpGet("search/{name}")]
		public async Task<IEnumerable<CostCenter>> SearchByDenom(string name)
		{
			IQueryable<CostCenter> query = _context.CostCenters;

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(entity => entity.Name.Contains(name));
			}

			return await query.ToListAsync();
		}

		// PUT: api/CostCenters/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCostCenter(int id, CostCenter costcenter)
		{
			if (id != costcenter.CostCenterId)
			{
				return BadRequest();
			}

			_context.Entry(costcenter).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CostCenterExists(id))
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

		// POST: api/CostCenters
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<CostCenter>> PostCostCenter(CostCenter costCenter)
		{
			if (_context.CostCenters == null)
			{
				return Problem("Entity set 'CorreosContext.CostCenters'  is null.");
			}
			_context.CostCenters.Add(costCenter);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCostCenter", new { id = costCenter.CostCenterId }, costCenter);
		}

		// bulk insert
		// POST: api/CostCenters/bulk
		[HttpPost("bulk")]
		public async Task<ActionResult<CostCenter>> PostCostCenters(IFormFile file)
		{
			if (_context.CostCenters == null)
			{
				return Problem("Entity set 'CuentasContext'  is null.");
			}

			var costcenters = _csvService.ReadCsvFile<CostCenter>(file.OpenReadStream()).ToList();

			foreach(var costcenter in costcenters) {
				costcenter.Date = _currentTimeService.GetCurrentTime();
			}

			_context.CostCenters.AddRange(costcenters);
			await _context.SaveChangesAsync();

			return CreatedAtAction("PostCostCenters", costcenters);
		}

		// DELETE: api/CostCenters/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCostCenter(int id)
		{
			if (_context.CostCenters == null)
			{
				return NotFound();
			}
			var costCenter = await _context.CostCenters.FindAsync(id);
			if (costCenter == null)
			{
				return NotFound();
			}

			_context.CostCenters.Remove(costCenter);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool CostCenterExists(int id)
		{
			return (_context.CostCenters?.Any(e => e.CostCenterId == id)).GetValueOrDefault();
		}
	}
}
