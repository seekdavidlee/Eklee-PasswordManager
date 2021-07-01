using Eklee.PasswordManager.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Data
{
	public interface IUserMetaDataService
	{
		Task<bool> Exist(ClaimsPrincipal claimsPrincipal);
		Task<UserMetaData> Get(ClaimsPrincipal claimsPrincipal);
		Task Save(ClaimsPrincipal claimsPrincipal, UserMetaData userMetaData);
	}
}
