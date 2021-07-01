using Azure.Identity;
using Azure.Storage.Blobs;
using Eklee.PasswordManager.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Data
{
	public class UserMetaDataService : IUserMetaDataService
	{
		private readonly string _containerName;
		private readonly string _storageName;
		public UserMetaDataService(IConfiguration configuration)
		{
			_containerName = configuration["KeyVaultName"];
			_storageName = configuration["StorageName"];
		}

		private BlobContainerClient _client;
		private async Task<BlobContainerClient> GetBlobContainerClient()
		{
			if (_client == null)
			{
				if (string.IsNullOrEmpty(_storageName))
				{
					_client = new BlobContainerClient("UseDevelopmentStorage=true", _containerName);
				}
				else
				{
					_client = new BlobContainerClient(new Uri($"https://{_storageName}.blob.core.windows.net/{_containerName}"), new DefaultAzureCredential());
				}

				await _client.CreateIfNotExistsAsync();

				return _client;
			}

			return _client;
		}

		public async Task Save(ClaimsPrincipal claimsPrincipal, UserMetaData userMetaData)
		{
			var blobClient = await GetUserBlobClient(claimsPrincipal);
			await blobClient.UploadAsync(BinaryData.FromString(JsonConvert.SerializeObject(userMetaData)), true);
		}

		public async Task<UserMetaData> Get(ClaimsPrincipal claimsPrincipal)
		{
			var blobClient = await GetUserBlobClient(claimsPrincipal);

			using var stream = new MemoryStream();
			var content = await blobClient.DownloadContentAsync();
			return JsonConvert.DeserializeObject<UserMetaData>(
				content.Value.Content.ToString());
		}

		public async Task<bool> Exist(ClaimsPrincipal claimsPrincipal)
		{
			return await (await GetUserBlobClient(claimsPrincipal)).ExistsAsync();
		}

		private async Task<BlobClient> GetUserBlobClient(ClaimsPrincipal claimsPrincipal)
		{
			var client = await GetBlobContainerClient();
			string blobName = $"{claimsPrincipal.UserId()}.json";
			var blobClient = client.GetBlobClient(blobName);

			return blobClient;
		}
	}
}
