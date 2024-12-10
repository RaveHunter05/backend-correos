using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using correos_backend.Attributes;

using Microsoft.AspNetCore.Authorization;

using correos_backend.Models.Enums;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BudgetsController : ControllerBase
	{
		private readonly CorreosContext _context;

		public BudgetsController(CorreosContext context)
		{
			_context = context;
		}

		// GET: api/Budgets
		[HttpGet]
		[JwtAuthorize("Boss", "User")]
		public async Task<ActionResult<IEnumerable<Budget>>> GetBudgets()
		{
			if (_context.Budgets == null)
			{
				return NotFound();
			}

			return  await _context.Budgets.Include(budget => budget.Comments).ToListAsync();
		}

		// GET: api/Budgets/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Budget>> GetBudget(int id)
		{
			if (_context.Budgets == null)
			{
				return NotFound();
			}
			var budget = await _context.Budgets.FindAsync(id);

			if (budget == null)
			{
				return NotFound();
			}

			return budget;
		}
		// GET: api/Budgets/search/names
		[HttpGet("search/{title}")]
		public async Task<IEnumerable<Budget>> SearchByName(string title)
		{
			IQueryable<Budget> query = _context.Budgets;

			if (!string.IsNullOrEmpty(title))
			{
				query = query.Where(budget => budget.Title.Contains(title));
			}

			return await query.ToListAsync();
		}

		// PUT: api/Budgets/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutBudget(int id, Budget budget)
		{
			if (id != budget.BudgetId)
			{
				return BadRequest();
			}

			_context.Entry(budget).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BudgetExists(id))
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

		// POST: api/Budgets
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Budget>> PostBudget(Budget budget)
		{

			if (_context.Budgets == null)
			{
				return Problem("Entity set 'CorreosContext.Budgets'  is null.");
			}


			if(budget.ApprovalStatus == null)
			{
				budget.ApprovalStatus = ApprovalStatus.Pending;
			}
			// @TODO: change later 
			if (budget.CreatedById == null)
			{
				budget.CreatedById = "1";
			}

			// @TODO Add document link to the budget
			_context.Budgets.Add(budget);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetBudget", new { id = budget.BudgetId }, budget);
		}

		// DELETE: api/Budgets/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBudget(int id)
		{
			if (_context.Budgets == null)
			{
				return NotFound();
			}
			var budget = await _context.Budgets.FindAsync(id);
			if (budget == null)
			{
				return NotFound();
			}

			_context.Budgets.Remove(budget);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool BudgetExists(int id)
		{
			return (_context.Budgets?.Any(e => e.BudgetId == id)).GetValueOrDefault();
		}
	}
}
