namespace correos_backend.Attributes

{
	public class JwtAuthorizeAttribute : Attribute
	{
		public string[] Roles { get; }
		public string Policy { get; }

		public JwtAuthorizeAttribute(params string[] roles)
		{
			Roles = roles;
		}

		public JwtAuthorizeAttribute(string policy)
		{
			Policy = policy;
		}
	}

}
