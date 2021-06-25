using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface IManagementClient
	{
		Task<bool> CanReadSecret(ClaimsPrincipal claimsPrincipal, string name);
	}
}
