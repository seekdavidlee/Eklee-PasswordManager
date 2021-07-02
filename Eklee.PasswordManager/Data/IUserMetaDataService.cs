using Eklee.PasswordManager.Core;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Data
{
	public interface IUserMetaDataService
	{
		Task<bool> Exist();
		Task<UserMetaData> Get();
		Task Save(UserMetaData userMetaData);
	}
}
