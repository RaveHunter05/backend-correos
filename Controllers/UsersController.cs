using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;

	public UsersController(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	[HttpGet]
	public async Task<ActionResult> GetUsers()
	{
		var users = await _userManager.Users.Select(group => new UsersViewModel()
		{
			UserName = group.UserName,
			Email = group.Email,
			Role = string.Join(",", _userManager.GetRolesAsync(group).Result.ToArray())
		}).ToListAsync();

		return Ok(users);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> GetUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user == null)
		{
			return NotFound("User not found");
		}
		return Ok(user);
	}

	// search by email
	[HttpGet("email/{email}")]
	public async Task<ActionResult> GetUserByEmail(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
		{
			return NotFound("User not found");
		}
		return Ok(user);
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user == null)
		{
			return NotFound("User not found");
		}
		var result = await _userManager.DeleteAsync(user);
		if (result.Succeeded)
		{
			return Ok("User deleted");
		}
		return BadRequest(result.Errors);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
	{
		var userToUpdate = await _userManager.FindByIdAsync(id);
		if (userToUpdate == null)
		{
			return NotFound("User not found");
		}
		userToUpdate.UserName = user.UserName;
		userToUpdate.Email = user.Email;
		var result = await _userManager.UpdateAsync(userToUpdate);
		if (result.Succeeded)
		{
			return Ok("User updated");
		}
		return BadRequest(result.Errors);
	}


	// PUT: api/users/disable/5
	// Disable a user
	[HttpPut("disable/{id}")]
	public async Task<ActionResult> DisableUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user == null)
		{
			return NotFound("User not found");
		}

		user.IsActive = false;
		user.LockoutEnabled = true;
		user.LockoutEnd = DateTime.Now.AddYears(100);
		var result = await _userManager.UpdateAsync(user);
		if (result.Succeeded)
		{
			return Ok("User disabled");
		}
		return BadRequest(result.Errors);
	}
}
