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
	public class ExpensesController : ControllerBase
	{
		private readonly CorreosContext _context;

		public ExpensesController(CorreosContext context)
		{
			_context = context;
		}

		// GET: api/Expenses
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
		{
			if (_context.Expenses == null)
			{
				return NotFound();
			}
			return await _context.Expenses.ToListAsync();
		}

		// GET: api/Expenses/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Expense>> GetExpense(int id)
		{
			if (_context.Expenses == null)
			{
				return NotFound();
			}
			var expense = await _context.Expenses.FindAsync(id);

			if (expense == null)
			{
				return NotFound();
			}

			return expense;
		}

		// GET: api/Expenses/search/name
		[HttpGet("search/{costCenter}")]
		public async Task<IEnumerable<Expense>> FindByService(string costCenter)
		{
			IQueryable<Expense> query = _context.Expenses;

			if (!string.IsNullOrEmpty(costCenter))
			{
				query = query.Include(i => i.CostCenter).Where(entity => entity.CostCenter.Name!.Contains(costCenter));
			}

			return await query.ToListAsync();
		}

		// PUT: api/Expenses/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutExpense(int id, Expense expense)
		{
			if (id != expense.ExpenseId)
			{
				return BadRequest();
			}

			_context.Entry(expense).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ExpenseExists(id))
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

		// POST: api/Expenses
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Expense>> PostExpense(Expense expense)
		{
			if (_context.Expenses == null)
			{
				return Problem("Entity set 'CorreosContext.Expenses'  is null.");
			}
			_context.Expenses.Add(expense);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetExpense", new { id = expense.ExpenseId }, expense);
		}

		// DELETE: api/Expenses/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExpense(int id)
		{
			if (_context.Expenses == null)
			{
				return NotFound();
			}
			var expense = await _context.Expenses.FindAsync(id);
			if (expense == null)
			{
				return NotFound();
			}

			_context.Expenses.Remove(expense);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ExpenseExists(int id)
		{
			return (_context.Expenses?.Any(e => e.ExpenseId == id)).GetValueOrDefault();
		}
	}
}
