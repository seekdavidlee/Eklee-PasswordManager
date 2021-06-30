using Microsoft.Edge.SeleniumTools;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Eklee.PasswordManager.Tests
{
	public abstract class BaseTest
	{
		protected IConfiguration Configuration { get; }
		private readonly string _applicationUrl;
		private readonly string _tenantId;

		public BaseTest()
		{
			var configuration = new ConfigurationBuilder();

			Configuration = configuration
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			_applicationUrl = Configuration["ApplicationUrl"];
			_tenantId = Configuration["TenantId"];
		}

		private UserProfile GetUserProfile(string name)
		{
			var list = Configuration.GetSection("UserProfiles")
				.Get<List<UserProfile>>();

			return list.Single(x => x.Name == name);
		}

		protected void LoginAndDoTest(Action<EdgeDriver> doTest, string userProfileName)
		{
			var userProfile = GetUserProfile(userProfileName);

			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Drivers");
			var options = new EdgeOptions
			{
				UseChromium = true
			};

			options.AddArgument("headless");
			options.AddArgument("disable-gpu");

			using (var edge = new EdgeDriver(path, options))
			{
				edge.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

				edge.Navigate().GoToUrl(_applicationUrl);
				edge.WaitForUrl($"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/authorize?client_id", WaitForUrlComparisons.StartsWith);

				var email = edge.FindElementByName("loginfmt");
				email.SendKeys(userProfile.Username);

				edge.FindAndClickElement("//input[@value = 'Next']");

				edge.FindAndTypeTextInElement("//input[@placeholder = 'Password']", userProfile.Password);

				edge.FindAndClickElement("//input[@value = 'Sign in']");

				edge.WaitForUrl($"https://login.microsoftonline.com/{_tenantId}/login", WaitForUrlComparisons.Equals);

				edge.FindAndClickElement("//input[@value = 'Next']");

				edge.WaitForUrl("https://mysignins.microsoft.com/register?csrf_token=", WaitForUrlComparisons.StartsWith);

				edge.FindAndClickElement("//a[text() = 'Skip setup']");

				var noStaySignedIn = edge.FindElementById("idBtn_Back");
				noStaySignedIn.Click();

				edge.WaitForUrl(_applicationUrl, WaitForUrlComparisons.StartsWith);

				doTest(edge);
			}
		}
	}
}
