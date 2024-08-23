using System.ComponentModel.DataAnnotations;

namespace correos_backend.Authentication
{
    public class ChangePasswordModel
    {
	[Required(ErrorMessage = "Email is required")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Old Password is required")]
	public string OldPassword { get; set; }

	[Required(ErrorMessage = "New Password is required")]
	public string NewPassword { get; set; }
    }
}

