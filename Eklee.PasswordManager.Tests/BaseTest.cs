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

				WebDriverWait wait = new(edge, TimeSpan.FromSeconds(5));
				wait.Until(x => x.Url.StartsWith($"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/authorize?client_id"));

				var email = edge.FindElementByName("loginfmt");
				email.SendKeys(userProfile.Username);

				var next = edge.FindElement(By.XPath("//input[@value = 'Next']"));
				next.Click();

				wait = new WebDriverWait(edge, TimeSpan.FromSeconds(5));
				wait.Until(x => SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//input[@placeholder = 'Password']")));

				var password = edge.FindElement(By.XPath("//input[@placeholder = 'Password']"));
				password.SendKeys(userProfile.Password);

				var signIn = edge.FindElement(By.XPath("//input[@value = 'Sign in']"));
				signIn.Click();

				wait = new WebDriverWait(edge, TimeSpan.FromSeconds(5));
				wait.Until(x => x.Url == $"https://login.microsoftonline.com/{_tenantId}/login");

				next = edge.FindElement(By.XPath("//input[@value = 'Next']"));
				next.Click();

				wait = new WebDriverWait(edge, TimeSpan.FromSeconds(5));
				wait.Until(x => x.Url.StartsWith("https://mysignins.microsoft.com/register?csrf_token="));

				wait = new WebDriverWait(edge, TimeSpan.FromSeconds(10));
				wait.Until(x => SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//a[text() = 'Skip setup']")));

				var skipSetup = edge.FindElement(By.XPath("//a[text() = 'Skip setup']"));
				skipSetup.Click();

				var noStaySignedIn = edge.FindElementById("idBtn_Back");
				noStaySignedIn.Click();

				wait = new WebDriverWait(edge, TimeSpan.FromSeconds(5));
				wait.Until(x => x.Url.StartsWith(""));

				doTest(edge);
			}
		}
	}
}
