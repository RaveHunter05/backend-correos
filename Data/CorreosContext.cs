using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class CorreosContext : IdentityDbContext<ApplicationUser>
{
	public CorreosContext (DbContextOptions<CorreosContext> options) : base(options)
	{
	}

	public DbSet<Spent> Spents {get; set;}
	public DbSet<Service> Services {get; set;}
	public DbSet<CostCenter> CostCenters {get; set;}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Income>()
			.HasOne<Service>(i => i.Service)
			.WithMany(s => s.Incomes)
			.HasForeignKey(i => i.ServiceId);


		modelBuilder.Entity<Income>()
			.HasOne<CostCenter>(i => i.CostCenter)
			.WithMany(s => s.Incomes)
			.HasForeignKey(i => i.CostCenterId);

		modelBuilder.Entity<Expense>()
			.HasOne<Spent>(i => i.Spent)
			.WithMany(s => s.Expenses)
			.HasForeignKey(i => i.SpentId);

		modelBuilder.Entity<Expense>()
			.HasOne<CostCenter>(i => i.CostCenter)
			.WithMany(s => s.Expenses)
			.HasForeignKey(i => i.CostCenterId);


		base.OnModelCreating(modelBuilder);
	}

	public DbSet<Income> Incomes {get; set;}
	public DbSet<Expense> Expenses {get; set;}
	public DbSet<Report> Reports {get; set;}
}
