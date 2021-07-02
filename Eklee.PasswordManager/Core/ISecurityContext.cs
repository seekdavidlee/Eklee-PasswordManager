using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	/// <summary>
	/// Security context is responsible for storing user authentication state and providing access to services that require it.
	/// </summary>
	public interface ISecurityContext
	{
		/// <summary>
		/// Configure authentication state so it can be used throughout.
		/// </summary>
		/// <param name="authenticationStateProvider">AuthenticationStateProvider.</param>
		Task Configure(AuthenticationStateProvider authenticationStateProvider);

		/// <summary>
		/// Gets the authenticated user id.
		/// </summary>
		/// <returns>string.</returns>
		string UserId();

		/// <summary>
		/// Checks if the user is authenticated.
		/// </summary>
		/// <returns>True if authenticated.</returns>
		bool IsAuthenticated();

		/// <summary>
		/// Gets user object id.
		/// </summary>
		/// <returns></returns>
		string UserObjectId();
	}
}
