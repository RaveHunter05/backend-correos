using correos_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace correos_backend.Repository
{
	public interface IJWTManagerRepository
	{
		Tokens Authenticate(Users users);
	}
}
