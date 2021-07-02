using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public interface IManagementClient
	{
		Task<bool> CanReadSecret(string name);
	}
}
