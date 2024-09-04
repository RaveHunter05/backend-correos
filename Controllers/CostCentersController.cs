using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers
{
	[Authorize (Roles = "Boss, Admin, Manager")]
	[Route("api/[controller]")]
	[ApiController]
	public class CostCentersController : ControllerBase
	{
		private readonly CorreosContext _context;

		public CostCentersController(CorreosContext context)
		{
			_context = context;
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
