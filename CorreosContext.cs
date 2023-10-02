using Microsoft.EntityFrameworkCore;
using correos_backend.Models;

public class CorreosContext : DbContext
{
	public CorreosContext (DbContextOptions<CorreosContext> options) : base(options)
	{
	}

	public DbSet<Users> Users {get; set;} 
}
