using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using correos_backend.Models;
using correos_backend.Repository;

[Authorize]
[Route("api/[controller]")]
[ApiController]  
public class UsersController : ControllerBase
{
	private readonly IJWTManagerRepository _jWTManager;

	public UsersController(IJWTManagerRepository jWTManager)
	{
		this._jWTManager = jWTManager;
	}

	[HttpGet]
	public List<string> Get()
	{
		var users = new List<string>
		{
			"Satinder Singh",
				"Amit Sarna",
				"Davin Jon"
		};

		return users;
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("authenticate")]
	public IActionResult Authenticate(Users usersdata)
	{
		var token = _jWTManager.Authenticate(usersdata);

		if (token == null)
		{
			return Unauthorized();
		}

		return Ok(token);
	}
}
