using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Core
{
	public class KeyVaultSecret
	{
		public string Id { get; set; }
	}

	public class KeyVaultSecretValue
	{
		public string Value { get; set; }
	}

	public class KeyVaultSecretList
	{
		[JsonProperty("value")]
		public List<KeyVaultSecret> Values { get; set; }
	}

	public class KeyVaultClient : IKeyVaultClient
	{
		private readonly IDownstreamWebApi _downstreamAPI;
		private const string _version = "api-version=7.1";

		public KeyVaultClient(IDownstreamWebApi downstreamAPI)
		{
			_downstreamAPI = downstreamAPI;
		}

		public async Task<IEnumerable<KeyVaultSecret>> ListSecrets()
		{
			var result = await _downstreamAPI.CallWebApiForUserAsync("KeyVault", x =>
			{
				x.RelativePath = $"secrets?{_version}";
			});

			result.EnsureSuccessStatusCode();

			var content = await result.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<KeyVaultSecretList>(content).Values;
		}

		public async Task<KeyVaultSecretValue> GetSecretValue(string name)
		{
			var result = await _downstreamAPI.CallWebApiForUserAsync("KeyVault", x =>
			{
				x.RelativePath = $"secrets/{name}?{_version}";
			});

			result.EnsureSuccessStatusCode();

			var content = await result.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<KeyVaultSecretValue>(content);
		}
	}
}
