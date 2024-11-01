using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
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
			var  expensesData = await _context.Expenses
				.GroupBy(expense => new {
						Date = expense.Date,
						ExpenseId = expense.ExpenseId,
						})
			.Select(group => new {
					Date = group.Key.Date,
					ExpenseId = group.Key.ExpenseId,
					CostCenter = group.Select(expense => new {
							CostCenterId = expense.CostCenterId,
							Name = expense.CostCenter!.Name,
							Code = expense.CostCenter!.Code
							}).FirstOrDefault(),
					Spent = group.Select(expense => new {
							SpentId = expense.SpentId,
							Category = expense.Spent!.Category,
							Denomination = expense.Spent!.Denomination
							}).FirstOrDefault(),
					ProjectedAmount = group.Sum(expense => expense.ProjectedAmount),
					ExecutedAmount = group.Sum(expense => expense.ExecutedAmount),
					})
			.OrderByDescending(x => x.Date)
				.ToListAsync();

			return Ok(expensesData);
		}

		// expenses inform by cost center
		// GET: api/Expenses by months, costcenter
		[HttpGet("month/costCenter/{initialDate}/{endDate}")]

			public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesMonthlyByCostCenter(DateTime initialDate, DateTime endDate)
			{
				if (_context.Expenses == null)
				{
					return NotFound();
				}
				var expensesByCostCenter = await _context.Expenses
					.Where(x => x.Date >= initialDate && x.Date <= endDate)
					.GroupBy(expense => new {
							Year = expense.Date.Year,
							CostCenterId = expense.CostCenterId,
							})
				.Select(group => new {
						CostCenterId = group.Key.CostCenterId,
						CostCenterInfo = group.GroupBy(costcenter  => new{
								Name = costcenter.CostCenter!.Name,
								Code = costcenter.CostCenter!.Code
								})
						.Select(x => new{
								Name = x.Key.Name,
								Code = x.Key.Code
								}).FirstOrDefault()
						,
						Year = group.Key.Year,
						Month = group.Select(expense => new {
								Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(expense.Date.Month),
								Projected = expense.ProjectedAmount,
								Executed = expense.ExecutedAmount
								})
						})
				.ToListAsync();

				var finalExpenses = expensesByCostCenter.Select(expense => new {
						CostCenterId = expense.CostCenterId,
						CostCenterInfo = expense.CostCenterInfo,
						Year = expense.Year,
						Months = expense.Month.GroupBy(m => m.Month)
						.Select(mGroup => new{
								Month = mGroup.Key,
								Executed = mGroup.Sum(m => m.Executed),
								Projected = mGroup.Sum(m => m.Projected)
								}),
						Absolute = expense.Month.Sum(x => x.Executed) - expense.Month.Sum(x => x.Projected),
						Percentual = expense.Month.Sum(x => x.Projected)  == 0 ? 0 : Math.Round(((expense.Month.Sum(x => x.Executed) - expense.Month.Sum(x => x.Projected)) / expense.Month.Sum(x => x.Projected) * 100), 2)

						});

				return Ok(finalExpenses);
			}


		// GET: api/General by months
		[HttpGet("month/general/{initialDate}/{endDate}")]

			public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesMonthlyGeneral(DateTime initialDate, DateTime endDate)
			{
				if (_context.Expenses == null)
				{
					return NotFound();
				}
				var expensesBySpent = await _context.Expenses
					.Where(x => x.Date >= initialDate && x.Date <= endDate)
					.GroupBy(expense => new {
							Year = expense.Date.Year,
							spentId = expense.SpentId,
							costCenterId = expense.CostCenterId
							})
				.Select(x => new {
						SpentId = x.Key.spentId,
						SpentInfo = x.GroupBy(expense => new{
								Category = expense.Spent!.Category,
								Denomination = expense.Spent!.Denomination
								})
						.Select(x => new{
								Category = x.Key.Category,
								Denomination = x.Key.Denomination
								}).FirstOrDefault()
						,
						CostCenterId = x.Key.costCenterId,
						CostCenterInfo = x.GroupBy(expense => new{
								Code = expense.CostCenter!.Code,
								Name = expense.CostCenter!.Name
								})
						.Select(x => new{
								Code = x.Key.Code,
								Name = x.Key.Name
								}).FirstOrDefault()
						,
							Year = x.Key.Year,
							Month = x.Select(expense => new {
									Month = expense.Date.Month,
									Executed = expense.ExecutedAmount,
									Projected = expense.ProjectedAmount
									})
				})
				.ToListAsync();

				var finalExpense = expensesBySpent.Select(expense => new{
						SpentId = expense.SpentId,
						SpentInfo = expense.SpentInfo,
						CostCenterId = expense.CostCenterId,
						CostCenterInfo = expense.CostCenterInfo,
						Months = expense.Month.GroupBy(x => x.Month)
						.Select(mGroup => new {
								Month = mGroup.Key,
								Executed = mGroup.Sum(m => m.Executed),
								Projected = mGroup.Sum(m => m.Projected),
								}),
						Absolute = expense.Month.Sum(m => m.Executed) - expense.Month.Sum(m => m.Projected),
						Percentual = expense.Month.Sum(m => m.Projected) == 0 ? 0 : Math.Round((((expense.Month.Sum(m => m.Executed) - expense.Month.Sum(m => m.Projected)) / expense.Month.Sum(m => m.Projected)) * 100) , 2)
						});

				return Ok(finalExpense);
			}

		// GET: api/Expenses by months, spent(gasto)
		[HttpGet("month/spent/{initialDate}/{endDate}")]

			public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesMonthlyBySpent(DateTime initialDate, DateTime endDate)
			{
				if (_context.Expenses == null)
				{
					return NotFound();
				}
				var expensesBySpent = await _context.Expenses
					.Where(x => x.Date >= initialDate && x.Date <= endDate)
					.GroupBy(expense => new {
							Year = expense.Date.Year,
							spentId = expense.SpentId,
							})
				.Select(x => new {
						SpentId = x.Key.spentId,
						SpentInfo = x.GroupBy(expense => new{
								Category = expense.Spent!.Category,
								Denomination = expense.Spent!.Denomination
								})
						.Select(x => new{
								Category = x.Key.Category,
								Denomination = x.Key.Denomination
								}).FirstOrDefault()
						,
						Year = x.Key.Year,
						Month = x.Select(expense => new {
								Month = expense.Date.Month,
								Executed = expense.ExecutedAmount,
								Projected = expense.ProjectedAmount
								})
						})
				.ToListAsync();

				var finalExpense = expensesBySpent.Select(expense => new{
						SpentId = expense.SpentId,
						SpentInfo = expense.SpentInfo,
						Months = expense.Month.GroupBy(x => x.Month)
						.Select(mGroup => new {
								Month = mGroup.Key,
								Executed = mGroup.Sum(m => m.Executed),
								Projected = mGroup.Sum(m => m.Projected),
								}),
						Absolute = expense.Month.Sum(m => m.Executed) - expense.Month.Sum(m => m.Projected),
						Percentual = expense.Month.Sum(m => m.Projected) == 0 ? 0 : Math.Round((((expense.Month.Sum(m => m.Executed) - expense.Month.Sum(m => m.Projected)) / expense.Month.Sum(m => m.Projected)) * 100) , 2)
						});

				return Ok(finalExpense);
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
				query = query.Include(i => i.CostCenter).Where(entity => entity.CostCenter.Name!.Contains(costCenter)).Include(x => x.Spent);
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
