using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface IKeyVaultClient
	{
		Task<IEnumerable<SecretItem>> ListSecrets(ClaimsPrincipal claimsPrincipal);
		Task<SecretValue> GetSecretValue(string name);
	}
}
