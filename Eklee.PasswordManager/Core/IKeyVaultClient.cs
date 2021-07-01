using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface IKeyVaultClient
	{
		Task<IEnumerable<KeyVaultSecret>> ListSecrets();
		Task<KeyVaultSecretValue> GetSecretValue(string name);
	}
}
