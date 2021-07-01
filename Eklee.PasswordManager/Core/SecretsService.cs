using Eklee.PasswordManager.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class SecretsService : ISecretsService
	{
		private readonly IKeyVaultClient _keyVaultClient;
		private readonly IManagementClient _managementClient;
		private readonly IUserMetaDataService _userMetaDataService;

		public SecretsService(
			IKeyVaultClient keyVaultClient,
			IManagementClient managementClient,
			IUserMetaDataService userMetaDataService)
		{
			_keyVaultClient = keyVaultClient;
			_managementClient = managementClient;
			_userMetaDataService = userMetaDataService;
		}

		public async Task<string> GetValue(string name)
		{
			var item = await _keyVaultClient.GetSecretValue(name);
			return item.Value;
		}

		public async Task Save(SecretItem secretItem, ClaimsPrincipal claimsPrincipal)
		{
			var meta = await GetUserMetaData(claimsPrincipal);

			if (meta.Items == null) meta.Items = new List<SecretMetaData>();

			var item = meta.Items.SingleOrDefault(x => x.Name == secretItem.Name);
			if (item == null)
			{
				item = new SecretMetaData { Name = secretItem.Name, DisplayName = secretItem.DisplayName };
				meta.Items.Add(item);
			}
			else
			{
				item.DisplayName = secretItem.DisplayName;
			}

			await _userMetaDataService.Save(claimsPrincipal, meta);
		}

		private async Task<UserMetaData> GetUserMetaData(ClaimsPrincipal claimsPrincipal)
		{
			UserMetaData meta;
			if (!await _userMetaDataService.Exist(claimsPrincipal))
			{
				meta = new UserMetaData { Created = System.DateTime.UtcNow };
				await _userMetaDataService.Save(claimsPrincipal, meta);
			}
			else
			{
				meta = await _userMetaDataService.Get(claimsPrincipal);
			}
			return meta;
		}

		public async Task<IEnumerable<SecretItem>> ListSecrets(ClaimsPrincipal claimsPrincipal)
		{
			var secrets = new List<SecretItem>();

			var meta = await GetUserMetaData(claimsPrincipal);

			foreach (var kvs in await _keyVaultClient.ListSecrets())
			{
				var parts = kvs.Id.Split('/');

				var item = new SecretItem();
				item.Name = parts[parts.Length - 1];

				SecretMetaData secretMetaData = null;
				if (meta.Items != null)
				{
					secretMetaData = meta.Items.SingleOrDefault(x => x.Name == item.Name);
					if (secretMetaData != null)
					{
						item.DisplayName = secretMetaData.DisplayName;
					}
				}

				if (secretMetaData == null) item.DisplayName = item.Name;

				if (await _managementClient.CanReadSecret(claimsPrincipal, item.Name))
				{
					secrets.Add(item);
				}
			}

			return secrets;
		}
	}
}
