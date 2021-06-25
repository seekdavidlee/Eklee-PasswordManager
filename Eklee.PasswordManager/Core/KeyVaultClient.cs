using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class KeyVaultClient : IKeyVaultClient
	{
		private readonly IManagementClient _managementClient;
		private readonly IDownstreamWebApi _downstreamAPI;
		private const string _version = "api-version=7.1";

		public KeyVaultClient(IManagementClient managementClient, IDownstreamWebApi downstreamAPI)
		{
			_managementClient = managementClient;
			_downstreamAPI = downstreamAPI;
		}

		public async Task<IEnumerable<SecretItem>> ListSecrets(ClaimsPrincipal claimsPrincipal)
		{
			var result = await _downstreamAPI.CallWebApiForUserAsync("KeyVault", x =>
			{
				x.RelativePath = $"secrets?{_version}";
			});

			result.EnsureSuccessStatusCode();

			var content = await result.Content.ReadAsStringAsync();

			var secretsResult = JsonConvert.DeserializeObject<SecretItemList>(content);

			var secrets = new List<SecretItem>();
			foreach (var item in secretsResult.Values)
			{
				if (await _managementClient.CanReadSecret(claimsPrincipal, item.Name))
				{
					secrets.Add(item);
				}
			}

			return secrets;
		}

		public async Task<SecretValue> GetSecretValue(string name)
		{
			var result = await _downstreamAPI.CallWebApiForUserAsync("KeyVault", x =>
			{
				x.RelativePath = $"secrets/{name}?{_version}";
			});

			result.EnsureSuccessStatusCode();

			var content = await result.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<SecretValue>(content);
		}
	}
}
