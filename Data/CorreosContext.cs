using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class CorreosContext : IdentityDbContext<ApplicationUser>
{
	public CorreosContext (DbContextOptions<CorreosContext> options) : base(options)
	{
	}

	public DbSet<Income> Incomes {get; set;}
	public DbSet<Expense> Expenses {get; set;}
}
