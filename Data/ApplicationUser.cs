using Microsoft.AspNetCore.Identity;

public class ApplicationUser: IdentityUser
{
	public bool IsActive { get; set; }
}
