using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;

namespace correos_backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}


	// GET: api/RoleController
	// To get all Roles
	//
	[HttpGet]
	public Task<IEnumerable<IdentityRole>> Get()
	{
		return Task.FromResult(_roleManager.Roles.AsEnumerable());
	}
	

	// POST: api/RoleController
	// To create a new Role
	[HttpPost]
	public async Task<ActionResult> Create(string Name)
	{
		if (string.IsNullOrEmpty(Name))
		{
			return BadRequest("Role name is required");
		}

		var role = new IdentityRole(Name);
		var result = await _roleManager.CreateAsync(role);

		if (result.Succeeded)
		{
			return Ok();
		}

		return BadRequest(result.Errors);
	}

	// PUT: api/RoleController/5/roleName
	// Add a user to a role
	[HttpPut("{userId}/{roleName}")]
	public async Task<ActionResult> AddUserToRole(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return NotFound("User not found");
		}

		var role = await _roleManager.FindByNameAsync(roleName);
		if (role == null)
		{
			return NotFound("Role not found");
		}

		var result = await _userManager.AddToRoleAsync(user, role.Name);

		if (result.Succeeded)
		{
			return Ok();
		}

		return BadRequest(result.Errors);
	}



	// PUT: api/RoleController/5/roleName
	// Change a user's role
	[HttpPut("change/{userId}/{roleName}")]
	public async Task<ActionResult> ChangeUserRole(string userId, string roleName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return NotFound("User not found");
		}

		var role = await _roleManager.FindByNameAsync(roleName);
		if (role == null)
		{
			return NotFound("Role not found");
		}

		var userRoles = await _userManager.GetRolesAsync(user);
		var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
		if (!result.Succeeded)
		{
			return BadRequest(result.Errors);
		}

		result = await _userManager.AddToRoleAsync(user, role.Name);
		if (result.Succeeded)
		{
			return Ok();
		}

		return BadRequest(result.Errors);
	}
}
