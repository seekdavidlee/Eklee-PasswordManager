using Eklee.PasswordManager.Data;
using System.Collections.Generic;
using System.Linq;
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

		public async Task Save(SecretItem secretItem)
		{
			var meta = await GetUserMetaData();

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

			await _userMetaDataService.Save(meta);
		}

		private async Task<UserMetaData> GetUserMetaData()
		{
			UserMetaData meta;
			if (!await _userMetaDataService.Exist())
			{
				meta = new UserMetaData { Created = System.DateTime.UtcNow };
			}
			else
			{
				meta = await _userMetaDataService.Get();
			}
			return meta;
		}

		public async Task<IEnumerable<SecretItem>> ListSecrets()
		{
			var secrets = new List<SecretItem>();

			var meta = await GetUserMetaData();

			foreach (var kvs in await _keyVaultClient.ListSecrets())
			{
				var parts = kvs.Id.Split('/');

				var item = new SecretItem
				{
					Name = parts[parts.Length - 1]
				};

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

				if (await _managementClient.CanReadSecret(item.Name))
				{
					secrets.Add(item);
				}
			}

			return secrets;
		}
	}
}
