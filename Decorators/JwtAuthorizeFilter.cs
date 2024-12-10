using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

using correos_backend.Attributes; 

namespace correos_backend.Decorators; 
public class JwtAuthorizeFilter : IAuthorizationFilter
{
    private readonly JwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandler;

    public JwtAuthorizeFilter(JwtSecurityTokenHandlerWrapper jwtSecurityTokenHandler)
    {
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Excluir rutas públicas
        var endpoint = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>().Any();
        
        if (endpoint) return;

        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.Replace("Bearer ", "").Trim();

        try
        {
            var claimsPrincipal = _jwtSecurityTokenHandler.ValidateJwtToken(token);
            var userId = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            var roles = claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            context.HttpContext.Items["UserId"] = userId;
            context.HttpContext.Items["Roles"] = roles;

	    context.HttpContext.User = claimsPrincipal;

            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Roles: {string.Join(",", roles)}");

            // Verificar roles y políticas
            var authorizeAttributes = context.ActionDescriptor.EndpointMetadata
                .OfType<JwtAuthorizeAttribute>();

            foreach (var attribute in authorizeAttributes)
            {
                if (attribute.Roles != null && !attribute.Roles.Any(role => roles.Contains(role)))
                {
                    context.Result = new ForbidResult(); 
                    return;
                }

                if (!string.IsNullOrEmpty(attribute.Policy))
                {
                    // Valida la política aquí según tu lógica
                    if (!ValidatePolicy(attribute.Policy, claimsPrincipal))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al validar el token: {ex.Message}");
            context.Result = new UnauthorizedResult();
        }
    }

    private bool ValidatePolicy(string policy, ClaimsPrincipal claimsPrincipal)
    {
        // Implementa la lógica de validación de políticas
        if (policy == "AdminOnly")
        {
            return claimsPrincipal.IsInRole("Admin");
        }
        return false;
    }
}
