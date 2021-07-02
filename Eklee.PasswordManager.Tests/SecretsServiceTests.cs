using Eklee.PasswordManager.Core;
using Eklee.PasswordManager.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
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

		[TestMethod]
		public async Task WhenUserMetaDataDoesNotExist_ItGetsCreatedWithItem()
		{
			_userMetaDataService.Exist().Returns(false);

			await _secretsService.Save(new SecretItem { DisplayName = "foo" });

			await _userMetaDataService.Received(1)
				.Save(Arg.Is<UserMetaData>(x => x.Items.Any(y => y.DisplayName == "foo") && x.Created.HasValue));
		}

		[TestMethod]
		public async Task WhenUserMetaDataExistWithoutItem_ItemGetsAdded()
		{
			var userMetaData = new UserMetaData
			{
				Created = DateTime.UtcNow,
				Items = new List<SecretMetaData>
				{
					new SecretMetaData { DisplayName = "app", Name = "bar" }
				}
			};

			_userMetaDataService.Exist().Returns(true);
			_userMetaDataService.Get().Returns(userMetaData);

			await _secretsService.Save(new SecretItem { DisplayName = "rat", Name = "foo" });

			Assert.AreEqual(2, userMetaData.Items.Count);
			Assert.AreEqual(userMetaData.Items[1].DisplayName, "rat");
			Assert.AreEqual(userMetaData.Items[1].Name, "foo");
		}

		[TestMethod]
		public async Task WhenUserMetaDataExistWitItem_ItemGetsUpdated()
		{
			var userMetaData = new UserMetaData
			{
				Created = DateTime.UtcNow,
				Items = new List<SecretMetaData>
				{
					new SecretMetaData { DisplayName = "app", Name = "bar" },
					new SecretMetaData { DisplayName = "bus", Name = "sop" }
				}
			};

			_userMetaDataService.Exist().Returns(true);
			_userMetaDataService.Get().Returns(userMetaData);

			await _secretsService.Save(new SecretItem { DisplayName = "car", Name = "sop" });

			Assert.AreEqual(2, userMetaData.Items.Count);
			Assert.AreEqual(userMetaData.Items[1].DisplayName, "car");
			Assert.AreEqual(userMetaData.Items[1].Name, "sop");
		}

		[TestMethod]
		public async Task ListOnlySecretsThatAreAuthorized()
		{
			_keyVaultClient.ListSecrets().Returns(new List<KeyVaultSecret>
			{
				new KeyVaultSecret{ Id="https://foo.com/abc1"},
				new KeyVaultSecret{ Id="https://foo.com/abc2"},
				new KeyVaultSecret{ Id="https://foo.com/abc3"}
			});

			_managementClient.CanReadSecret(Arg.Is("abc1")).Returns(true);
			_managementClient.CanReadSecret(Arg.Is("abc2")).Returns(false);
			_managementClient.CanReadSecret(Arg.Is("abc3")).Returns(true);

			var secrets = (await _secretsService.ListSecrets()).ToList();

			Assert.AreEqual(2, secrets.Count);
			Assert.AreEqual("abc1", secrets[0].Name);
			Assert.AreEqual("abc3", secrets[1].Name);
		}
	}
}
