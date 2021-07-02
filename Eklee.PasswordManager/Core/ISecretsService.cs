using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface ISecretsService
	{
		Task<string> GetValue(string name);
		Task<IEnumerable<SecretItem>> ListSecrets();
		Task Save(SecretItem secretIteml);
	}
}
