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
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IConfiguration _configuration;
	private readonly JwtSecurityTokenHandlerWrapper _jwtHelper;

	public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, JwtSecurityTokenHandlerWrapper jwtHelper)
	{
		_userManager = userManager;
		_roleManager = roleManager;
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
				var roles = await _userManager.GetRolesAsync(user);
				var token =  _jwtHelper.GenerateJwtToken(user.Id, user.Email);
				return Ok(new {token, user, roles});
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

		Console.WriteLine(result);
		if (result.Succeeded)
		{
 var defaultrole = _roleManager.FindByNameAsync("Default").Result;  
 if (defaultrole != null)
 {
	 IdentityResult roleResult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
 }
			return Ok(new Response { Status = "Success", Message = "User created successfully!" });
		}
		return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

	}

	[HttpPut]
	[Route("changepassword")]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
	{
		if (!ModelState.IsValid)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Invalid model" });

		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found" });

		var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
		if (result.Succeeded)
			return Ok(new Response { Status = "Success", Message = "Password changed successfully!" });

		return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Password change failed! Please check password details and try again." });
	}

	[HttpPut]
	[Route("changerole")]
	public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleModel model)
	{
		if (!ModelState.IsValid)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Invalid model" });

		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User not found" });

		var roles = await _userManager.GetRolesAsync(user);
		var result = await _userManager.RemoveFromRolesAsync(user, roles);
		if (!result.Succeeded)
			return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role change failed! Please check role details and try again." });

		result = await _userManager.AddToRoleAsync(user, model.Role);
		if (result.Succeeded)
			return Ok(new Response { Status = "Success", Message = "Role changed successfully!" });

		return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role change failed! Please check role details and try again." });
	}

}
