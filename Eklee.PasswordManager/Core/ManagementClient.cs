using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class ManagementClient : IManagementClient
	{
		// https://docs.microsoft.com/en-us/azure/role-based-access-control/role-assignments-list-rest

		private const string ManagementUri = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.KeyVault/vaults/{2}/secrets/_secret_/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01&$filter=assignedTo('_objectId_')";
		private readonly string _url;
		private readonly IDownstreamWebApi _downstreamAPI;
		private readonly ISecurityContext _securityContext;

		public ManagementClient(
			IConfiguration configuration,
			IDownstreamWebApi downstreamAPI,
			ISecurityContext securityContext)
		{
			var keyVaultName = configuration["KeyVaultName"];
			var subscriptionId = configuration["Management:SubscriptionId"];
			var resourceGroupName = configuration["Management:ResourceGroupName"];

			_url = string.Format(ManagementUri, subscriptionId, resourceGroupName, keyVaultName);
			_downstreamAPI = downstreamAPI;
			_securityContext = securityContext;
		}

		public async Task<bool> CanReadSecret(string name)
		{
			var objectId = _securityContext.UserObjectId();

			var result = await _downstreamAPI.CallWebApiForUserAsync("Management", x =>
			{
				var res = _url.Replace("_secret_", name).Replace("_objectId_", objectId);
				x.RelativePath = res;
			});

			result.EnsureSuccessStatusCode();

			var content = await result.Content.ReadAsStringAsync();

			return CanAccessKeyVaultSecret(JsonConvert.DeserializeObject<RoleAssignmentResult>(content));
		}

		private static bool CanAccessKeyVaultSecret(RoleAssignmentResult result)
		{
			return result.Values.Any(x =>
			{
				var parts = x.Role.RoleDefinitionId.Split('/');
				var id = parts[^1];
				return id == MyConstants.KeyVaultUserSecretRoleDefinationId ||
					   id == MyConstants.KeyVaultAdministratorRoleDefinationId;
			});
		}
	}
}
