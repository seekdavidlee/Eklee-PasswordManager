using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class SecurityContext : ISecurityContext
	{
		private AuthenticationState _authenticationState;

		public async Task Configure(AuthenticationStateProvider authenticationStateProvider)
		{
			_authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
		}

		public bool IsAuthenticated()
		{
			return _authenticationState.User.Identity.IsAuthenticated;
		}

		public string UserId()
		{
			return GetClaimValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
		}

		public string UserObjectId()
		{
			return GetClaimValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
		}

		private string GetClaimValue(string type)
		{
			return _authenticationState.User.Claims.Single(x => x.Type == type).Value;
		}
	}
}
