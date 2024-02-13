using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IncomesController : ControllerBase
	{
		private readonly CorreosContext _context;

		public IncomesController(CorreosContext context)
		{
			_context = context;
		}

		// GET: api/Incomes
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Income>>> GetIncomes()
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}
			return await _context.Incomes.Include(s => s.Service).Include(n => n.CostCenter).ToListAsync();
		}

		// GET: api/Incomes/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Income>> GetIncome(int id)
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}
			var income = await _context.Incomes.FindAsync(id);

			if (income == null)
			{
				return NotFound();
			}

			return income;
		}

		// GET: api/Incomes/search/name
		[HttpGet("search/{service}")]
		public async Task<IEnumerable<Income>> FindByService(string service)
		{
			IQueryable<Income> query = _context.Incomes;

			if (!string.IsNullOrEmpty(service))
			{
				query = query.Include(i => i.Service).Where(entity => entity.Service!.Name.Contains(service));
			}

			return await query.ToListAsync();
		}

		// PUT: api/Incomes/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutIncome(int id, Income income)
		{
			if (id != income.IncomeId)
			{
				return BadRequest();
			}

			_context.Entry(income).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!IncomeExists(id))
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

		// POST: api/Incomes
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Income>> PostIncome(Income income)
		{
			if (_context.Incomes == null)
			{
				return Problem("Entity set 'CorreosContext.Incomes'  is null.");
			}
			_context.Incomes.Add(income);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetIncome", new { id = income.IncomeId }, income);
		}

		// DELETE: api/Incomes/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteIncome(int id)
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}
			var income = await _context.Incomes.FindAsync(id);
			if (income == null)
			{
				return NotFound();
			}

			_context.Incomes.Remove(income);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool IncomeExists(int id)
		{
			return (_context.Incomes?.Any(e => e.IncomeId == id)).GetValueOrDefault();
		}
	}
}
