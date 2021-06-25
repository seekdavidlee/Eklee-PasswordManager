using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class ManagementClient : IManagementClient
	{
		// https://docs.microsoft.com/en-us/azure/role-based-access-control/role-assignments-list-rest

		private const string ManagementUri = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.KeyVault/vaults/{2}/secrets/_secret_/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01&$filter=assignedTo('_objectId_')";
		private readonly string _url;
		private readonly IDownstreamWebApi _downstreamAPI;

		public ManagementClient(
			IConfiguration configuration,
			IDownstreamWebApi downstreamAPI)
		{
			var keyVaultName = configuration["KeyVaultName"];
			var subscriptionId = configuration["Management:SubscriptionId"];
			var resourceGroupName = configuration["Management:ResourceGroupName"];

			_url = string.Format(ManagementUri, subscriptionId, resourceGroupName, keyVaultName);
			_downstreamAPI = downstreamAPI;
		}

		public async Task<bool> CanReadSecret(ClaimsPrincipal claimsPrincipal, string name)
		{
			var objectId = claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

			if (string.IsNullOrEmpty(objectId))
			{
				throw new Exception("Invalid user object id");
			}

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
