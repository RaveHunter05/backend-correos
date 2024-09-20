using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace correos_backend.Decorators; // Replace with your project's namespace
public class JwtAuthorizeFilter : IAuthorizationFilter
{
    private readonly JwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandler;

    public JwtAuthorizeFilter(JwtSecurityTokenHandlerWrapper jwtSecurityTokenHandler){
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the [Authorize] attribute is explicitly applied to the action or controller.
        var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AuthorizeAttribute);

	// no llega hasta aquí
    	Console.BackgroundColor = ConsoleColor.DarkGreen;
	Console.WriteLine("Authorize Attribute: " + hasAuthorizeAttribute);

        if (hasAuthorizeAttribute)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Validate the token and extract claims
                    var claimsPrincipal = _jwtSecurityTokenHandler.ValidateJwtToken(token);

                    // Extract the user ID from the token
                    var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    context.HttpContext.Items["UserId"] = userId;
                }
                catch (Exception)
                {
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("User Unauthorized");
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

}
