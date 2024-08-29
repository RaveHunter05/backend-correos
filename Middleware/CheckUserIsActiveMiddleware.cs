using Microsoft.AspNetCore.Identity;

namespace correos_backend.Middleware;

public class CheckUserIsActiveMiddleware
{
	private readonly RequestDelegate _next;

	public CheckUserIsActiveMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager)
	{
		var user = context.User;
		if (user.Identity.IsAuthenticated)
		{
			var applicationUser = await userManager.FindByEmailAsync(user.Identity.Name);
			if (applicationUser != null && !applicationUser.IsActive)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync("User account is not active");
				return;
			}
		}
		await _next(context);
	}
}
