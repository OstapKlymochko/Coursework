using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace AuthService.Helpers
{
	public static class IdentityResultExtension
	{
		public static string MapErrors(this IdentityResult result)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} : {1}", "Failed", string.Join(", ", result.Errors.Select(x => x.Description).ToList()));
		}
	}
}
