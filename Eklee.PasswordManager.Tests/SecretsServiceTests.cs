using Eklee.PasswordManager.Core;
using Eklee.PasswordManager.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;

namespace Eklee.PasswordManager.Tests
{
	[TestCategory(Constants.TestCategory.Unit)]
	[TestClass]
	public class SecretsServiceTests
	{
		private readonly SecretsService _secretsService;
		private readonly IKeyVaultClient _keyVaultClient;
		private readonly IManagementClient _managementClient;
		private readonly IUserMetaDataService _userMetaDataService;
		public SecretsServiceTests()
		{
			_keyVaultClient = Substitute.For<IKeyVaultClient>();
			_managementClient = Substitute.For<IManagementClient>();
			_userMetaDataService = Substitute.For<IUserMetaDataService>();
			_secretsService = new SecretsService(_keyVaultClient, _managementClient, _userMetaDataService);
		}

		[TestMethod]
		public async Task ReturnValue()
		{
			_keyVaultClient.GetSecretValue(Arg.Is("secret1"))
				.Returns(new KeyVaultSecretValue { Value = "foobar" });

			var value = await _secretsService.GetValue("secret1");
			Assert.AreEqual("foobar", value);
		}
	}
}
