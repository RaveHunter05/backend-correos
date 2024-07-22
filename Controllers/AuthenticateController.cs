using correos_backend.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace correos_backend.Controllers;

[ApiController]
public class AuthenticateController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IConfiguration _configuration;
	private readonly JwtSecurityTokenHandlerWrapper _jwtHelper;

	public AuthenticateController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, JwtSecurityTokenHandlerWrapper jwtHelper)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_configuration = configuration;
		_jwtHelper = jwtHelper;
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		if (ModelState.IsValid)
		{
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

			if(result.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				var token =  _jwtHelper.GenerateJwtToken(user.Id, user.Email);
				return Ok(new {token, user});
			}
			return Unauthorized();

		}
		return BadRequest(ModelState);
	}

	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register([FromBody] RegisterModel model)
	{
		if (!ModelState.IsValid)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

		ApplicationUser user = new ApplicationUser()
		{
			Email = model.Email,
			      SecurityStamp = Guid.NewGuid().ToString(),
			      UserName = model.Username
		};
		var result = await _userManager.CreateAsync(user, model.Password);
		if (result.Succeeded)
		{
			return Ok(new Response { Status = "Success", Message = "User created successfully!" });
		}
		return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

	}
}
