using System.Linq;
using System.Security.Claims;

namespace Eklee.PasswordManager.Core
{
	public static class Extensions
	{
		public static string UserId(this ClaimsPrincipal claimsPrincipal)
		{
			var id = claimsPrincipal.Claims.Single(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
			return id;
		}
	}
}
