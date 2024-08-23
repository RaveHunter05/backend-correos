using System.ComponentModel.DataAnnotations;

namespace correos_backend.Authentication
{
    public class ChangeRoleModel
    {
	[Required(ErrorMessage = "Email is required")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Role is required")]
	public string Role { get; set; }
    }
}

