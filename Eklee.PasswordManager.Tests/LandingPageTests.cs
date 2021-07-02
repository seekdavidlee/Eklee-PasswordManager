using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eklee.PasswordManager.Tests
{
	[TestCategory(Constants.TestCategory.Functional)]
	[TestClass]
	public class LandingPageTests : BaseTest
	{
		[TestMethod]
		public void PartialSecretsAssignedToUser_UserSeesAssignedSecrets()
		{
			LoginAndDoTest(edge =>
			{
				edge.AssertTableColumnTextIsMissing("app1secret1");
				edge.AssertTableColumnTextIsMissing("app1secret2");

				edge.AssertTableColumnTextIsPresent("app2secret1");
				edge.AssertTableColumnTextIsPresent("app2secret2");
			}, Constants.AutomationAccount1);
		}

		[TestMethod]
		public void NoSecretsAssignedToUser_UserDoNotSeeAnySecrets()
		{
			LoginAndDoTest(edge =>
			{
				edge.AssertTableColumnTextIsMissing("app1secret1");
				edge.AssertTableColumnTextIsMissing("app1secret2");

				edge.AssertTableColumnTextIsMissing("app2secret1");
				edge.AssertTableColumnTextIsMissing("app2secret2");
			}, Constants.AutomationAccount2);
		}

		[TestMethod]
		public void AllSecretsAssignedToUser_UserSeesAllSecrets()
		{
			LoginAndDoTest(edge =>
			{
				edge.AssertTableColumnTextIsPresent("app1secret1");
				edge.AssertTableColumnTextIsPresent("app1secret2");

				edge.AssertTableColumnTextIsPresent("app2secret1");
				edge.AssertTableColumnTextIsPresent("app2secret2");
			}, Constants.AutomationAccount3);
		}
	}
}
