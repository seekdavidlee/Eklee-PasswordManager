using Azure.Identity;
using Azure.Storage.Blobs;
using Eklee.PasswordManager.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Data
{
	public class UserMetaDataService : IUserMetaDataService
	{
		private readonly string _containerName;
		private readonly string _storageName;
		private readonly ISecurityContext _securityContext;

		public UserMetaDataService(IConfiguration configuration, ISecurityContext securityContext)
		{
			_containerName = configuration["KeyVaultName"];
			_storageName = configuration["StorageName"];
			_securityContext = securityContext;
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

		public async Task Save(UserMetaData userMetaData)
		{
			var blobClient = await GetUserBlobClient();
			await blobClient.UploadAsync(BinaryData.FromString(JsonConvert.SerializeObject(userMetaData)), true);
		}

		public async Task<UserMetaData> Get()
		{
			var blobClient = await GetUserBlobClient();

			using var stream = new MemoryStream();
			var content = await blobClient.DownloadContentAsync();
			return JsonConvert.DeserializeObject<UserMetaData>(
				content.Value.Content.ToString());
		}

		public async Task<bool> Exist()
		{
			return await (await GetUserBlobClient()).ExistsAsync();
		}

		private async Task<BlobClient> GetUserBlobClient()
		{
			var client = await GetBlobContainerClient();
			string blobName = $"{_securityContext.UserId()}.json";
			var blobClient = client.GetBlobClient(blobName);

			return blobClient;
		}
	}
}
