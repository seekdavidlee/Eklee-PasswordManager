using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eklee.PasswordManager.Tests
{
	[TestClass]
	public class LandingPageTests : BaseTest
	{
		[TestMethod]
		public void AssignedSecretsExistForUserWithPartialAccess()
		{
			LoginAndDoTest(edge =>
			{
				edge.AssertTableColumnTextIsMissing("app1secret1");
				edge.AssertTableColumnTextIsMissing("app1secret2");

				edge.AssertTableColumnTextIsPresent("app2secret1");
				edge.AssertTableColumnTextIsPresent("app2secret2");
			}, Constants.AutomationAccount1);
		}
	}
}
