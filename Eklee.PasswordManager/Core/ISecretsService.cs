using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface ISecretsService
	{
		Task<string> GetValue(string name);
		Task<IEnumerable<SecretItem>> ListSecrets(ClaimsPrincipal claimsPrincipal);
		Task Save(SecretItem secretItem, ClaimsPrincipal claimsPrincipal);
	}
}
