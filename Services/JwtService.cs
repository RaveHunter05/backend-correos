using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace correos_backend.Services;

public class JwtService
{
	private readonly IConfiguration _configuration;

	public JwtService(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<string> GenerateToken(string userEmail)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Email, userEmail),
			    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				    // Add additional claims as needed
		};

		var jwtSettings = _configuration.GetSection("JwtSettings");
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["TokenLifetime"]));

		var token = new JwtSecurityToken(
				jwtSettings["Issuer"],
				jwtSettings["Audience"],
				claims,
				expires: expires,
				signingCredentials: credentials
				);

		var tokenHandler = new JwtSecurityTokenHandler();
		return tokenHandler.WriteToken(token);
	}
}
