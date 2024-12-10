using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;

	public UsersController(UserManager<IdentityUser> userManager)
	{
		_userManager = userManager;
	}

	[HttpGet]
	public async Task<ActionResult> GetUsers()
	{
		var users = await _userManager.Users.Select(user => new UsersViewModel
				{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				}).ToListAsync();

		foreach (var user in users)
		{
			var role = await _userManager.GetRolesAsync(new IdentityUser { Id = user.Id });
			user.Role = string.Join(",", role);
		}

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

		IQueryable<IdentityUser> query = _userManager.Users;

		if (!string.IsNullOrEmpty(email))
		{
			query = query.Where(entity => entity.Email.Contains(email));
		}
		return Ok(await query.ToListAsync());
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
	public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserModel user)
	{
		var userToUpdate = await _userManager.FindByIdAsync(id);
		if (userToUpdate == null)
		{
			return NotFound("User not found");
		}
		if (user.UserName != null)
		{
			userToUpdate.UserName = user.UserName;
		}
		if (user.Email != null)
		{
			userToUpdate.Email = user.Email;
		}
		var result = await _userManager.UpdateAsync(userToUpdate);

		if (result.Succeeded)
		{
			if (user.Role != null)
			{
				var roles = await _userManager.GetRolesAsync(userToUpdate);
				await _userManager.RemoveFromRolesAsync(userToUpdate, roles);
				await _userManager.AddToRoleAsync(userToUpdate, user.Role);
			}
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

		if (user.UserName == "admin")
		{
			return BadRequest("You can't disable the admin user");
		}

		user.LockoutEnabled = true;
		user.LockoutEnd = DateTime.Now.AddYears(100);
		var result = await _userManager.UpdateAsync(user);
		if (result.Succeeded)
		{
			return Ok("User disabled");
		}
		return BadRequest(result.Errors);
	}

	// PUT: api/users/enable/5
	// Enable a user
	[HttpPut("enable/{id}")]
	public async Task<ActionResult> EnableUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user == null)
		{
			return NotFound("User not found");
		}


		user.LockoutEnabled = false;
		user.LockoutEnd = null;
		var result = await _userManager.UpdateAsync(user);
		if (result.Succeeded)
		{
			return Ok("User enabled");
		}
		return BadRequest(result.Errors);
	}
}
