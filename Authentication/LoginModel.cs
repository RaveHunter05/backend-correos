using System.ComponentModel.DataAnnotations;

namespace correos_backend.Authentication
{
	public class LoginModel
	{
		[Required(ErrorMessage = "User Name is required")]
		public string Username {get; set;}

		[Required(ErrorMessage = "Email is required")]
		public string Email {get; set;}

		[Required(ErrorMessage = "Password is required")]
		public string Password {get; set;}
	}
}
