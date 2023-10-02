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

using correos_backend.Services;

namespace correos_backend.Controllers;

[ApiController]
public class AuthenticateController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IConfiguration _configuration;
	private readonly JwtService _jwtService;

	public AuthenticateController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, JwtService jwtService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_configuration = configuration;
		_jwtService = jwtService;
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		if (ModelState.IsValid)
		{
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

			if(result.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				var token = await _jwtService.GenerateToken(model.Email);
				return Ok(new {token});
			}
		}
		return Unauthorized();
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
		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

		return Ok(new Response { Status = "Success", Message = "User created successfully!" });
	}
}
