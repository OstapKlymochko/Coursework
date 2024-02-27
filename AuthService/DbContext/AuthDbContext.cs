using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DbContext
{
	public class AuthDbContext: IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
	{
		public AuthDbContext(DbContextOptions options): base(options) 
		{
		}
	}
}
