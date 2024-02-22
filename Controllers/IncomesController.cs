using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

		// GET: api/Incomes/month/service/2023
		[HttpGet("month/service/{year}")]
		public async Task<ActionResult<IEnumerable<object>>> GetIncomesMonthlyByService(int year, [FromQuery] int[] months)
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}

			var incomesByService = await _context.Incomes
				.Where(s => s.Date.Year == year)
				.Where(x => months.Contains(x.Date.Month))
				.GroupBy(income => new
						{
						Year = income.Date.Year,
						ServiceId = income.ServiceId,
						})
			.Select(group => new
					{
					ServiceId = group.Key.ServiceId,
					ServiceInfo = group.GroupBy(service => new {
							Name = service.Service!.Name,
							Code = service.Service!.Code
							}).Select(x => new {
								Name = x.Key.Name,
								Code = x.Key.Code
								}).FirstOrDefault(),
					Months = group.Select(income => new
							{
							Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(income.Date.Month),
							Executed = income.ExecutedAmount,
							Projected = income.ProjectedAmount
							}),
					})
			.ToListAsync();

			var incomesFinal = incomesByService.Select(group => new
					{
					ServiceId = group.ServiceId,
					ServiceInfo = group.ServiceInfo,
					Months = group.Months.GroupBy(m => m.Month)
					.Select(mGroup => new
							{
							Month = mGroup.Key,
							Executed = mGroup.Sum(m => m.Executed),
							Projected = mGroup.Sum(m => m.Projected),
							}),
					Absolute = group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected),
					Percentual = Math.Round((((group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected)) / group.Months.Sum(m => m.Projected)) * 100), 2),
					});

			return Ok(incomesFinal);
		}


		// GET: api/Incomes/month/general/2023
		// get last month 
		[HttpGet("month/general/{year}")]
		public async Task<ActionResult<IEnumerable<Income>>> GetIncomsMonthlyGeneral(int year, [FromQuery] int[] months)
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}
			var incomesByCostCenter = await _context.Incomes
				.Where(s => s.Date.Year == year)
				.Where(x => months.Contains(x.Date.Month))
				.GroupBy(income => new {
						Year = income.Date.Year,
						CostCenterId = income.CostCenterId,
						ServiceId = income.ServiceId
						})
			.Select(x => new {
					CostCenterId = x.Key.CostCenterId,
					CostCenterInfo = x.GroupBy(costcenter => new {
							Name = costcenter.CostCenter!.Name,
							Code = costcenter.CostCenter!.Code
							})
					.Select(x => new{
							Name = x.Key.Name,
							Code = x.Key.Code
							}).FirstOrDefault(),
					ServiceId = x.Key.ServiceId,
					ServiceInfo = x.GroupBy(service => new {
							Name = service.Service!.Name,
							Code = service.Service!.Code,
							})
					.Select(x => new{
							Name = x.Key.Name,
							Code = x.Key.Code
							}).FirstOrDefault(),
					Months = x.Select(income => new {
							Month = income.Date.Month,
							Executed = income.ExecutedAmount,
							Projected = income.ProjectedAmount
							}),
					})
			.ToListAsync();

			var incomesFinal = incomesByCostCenter.Select(group => new 
					{
					CostCenterId = group.CostCenterId,
					CostCenterInfo = group.CostCenterInfo,
					ServiceId = group.ServiceId,
					ServiceInfo = group.ServiceInfo,
					Months = group.Months.GroupBy(m => m.Month)
					.Select(mGroup => new{
							Month = mGroup.Key,
							Executed = mGroup.Sum(m => m.Executed),
							Projected = mGroup.Sum(m => m.Projected)
							}),
					Absolute = group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected),
					Percentual = Math.Round((((group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected)) / group.Months.Sum(m => m.Projected)) * 100), 2)
					}
					);

			return Ok(incomesFinal);
		}

		// GET: api/Incomes/month/costcenter/2023
		// get last month 
		[HttpGet("month/costcenter/{year}")]
		public async Task<ActionResult<IEnumerable<Income>>> GetIncomsMonthlyByCostCenter(int year, [FromQuery] int[] months)
		{
			if (_context.Incomes == null)
			{
				return NotFound();
			}
			var incomesByCostCenter = await _context.Incomes
				.Where(s => s.Date.Year == year)
				.Where(x => months.Contains(x.Date.Month))
				.GroupBy(income => new {
						Year = income.Date.Year,
						CostCenterId = income.CostCenterId,
						})
			.Select(x => new {
					CostCenterId = x.Key.CostCenterId,
					CostCenterInfo = x.GroupBy(costcenter => new {
							Name = costcenter.CostCenter!.Name,
							Code = costcenter.CostCenter!.Code
							})
					.Select(x => new{
							Name = x.Key.Name,
							Code = x.Key.Code
							}).FirstOrDefault(),
					Months = x.Select(income => new {
							Month = income.Date.Month,
							Executed = income.ExecutedAmount,
							Projected = income.ProjectedAmount
							}),
					})
			.ToListAsync();

			var incomesFinal = incomesByCostCenter.Select(group => new 
					{
					CostCenterId = group.CostCenterId,
					CostCenterInfo = group.CostCenterInfo,
					Months = group.Months.GroupBy(m => m.Month)
					.Select(mGroup => new{
							Month = mGroup.Key,
							Executed = mGroup.Sum(m => m.Executed),
							Projected = mGroup.Sum(m => m.Projected)
							}),
					Absolute = group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected),
					Percentual = Math.Round((((group.Months.Sum(m => m.Executed) - group.Months.Sum(m => m.Projected)) / group.Months.Sum(m => m.Projected)) * 100), 2)
					}
					);

			return Ok(incomesFinal);
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
