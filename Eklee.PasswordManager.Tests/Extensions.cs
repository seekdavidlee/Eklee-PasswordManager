using Microsoft.Edge.SeleniumTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Reflection;

namespace Eklee.PasswordManager.Tests
{
	public static class Extensions
	{
		public static void AssertTableColumnTextIsPresent(this EdgeDriver edge, string text)
		{
			var e = edge.FindElement(By.XPath($"//td/div[text() = '{text}']"));
			Assert.IsNotNull(e);
		}

		public static void AssertTableColumnTextIsMissing(this EdgeDriver edge, string text)
		{
			Assert.ThrowsException<NoSuchElementException>(() =>
			{
				var e = edge.FindElement(By.XPath($"//td/div[text() = '{text}']"));
			});
		}

		public static void FindAndClickElement(this EdgeDriver edge, string xPath)
		{
			try
			{
				var wait = new WebDriverWait(edge, TimeSpan.FromSeconds(10));
				wait.Until(x => SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));

				var element = edge.FindElement(By.XPath(xPath));
				element.Click();
			}
			catch (NoSuchElementException)
			{
				var filePath = edge.SaveScreenshot();
				TestLogger.WriteLog($"Screenshot saved to {filePath}.");
				throw;
			}
		}

		public static void FindAndTypeTextInElement(this EdgeDriver edge, string xPath, string text)
		{
			var wait = new WebDriverWait(edge, TimeSpan.FromSeconds(10));
			wait.Until(x => SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));

			var element = edge.FindElement(By.XPath(xPath));
			element.SendKeys(text);
		}

		public static void WaitForUrl(this EdgeDriver edge, string url, WaitForUrlComparisons waitForUrlComparison)
		{
			var wait = new WebDriverWait(edge, TimeSpan.FromSeconds(15));

			try
			{
				wait.Until(x =>
				{

					switch (waitForUrlComparison)
					{
						case WaitForUrlComparisons.Equals:
							return x.Url == url;

						case WaitForUrlComparisons.StartsWith:
							return x.Url.StartsWith(url);

						default:
							throw new NotImplementedException();
					}

				});
			}
			catch (WebDriverTimeoutException)
			{
				TestLogger.WriteLog($"Expected {url} but url is {edge.Url}.");
				edge.SaveScreenshot();
				throw;
			}
		}

		/// <summary>
		/// Saves a screenshot and returns the path of where screenshot was saved.
		/// </summary>
		/// <param name="edge">EdgeDriver.</param>
		/// <returns>File path to where screenshot was saved.</returns>
		public static string SaveScreenshot(this EdgeDriver edge)
		{
			string fileName = $"{DateTime.UtcNow.ToString("yyyyMMddhhmmss")}.png";
			var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
			edge.GetScreenshot().SaveAsFile(filePath);
			return filePath;
		}
	}

	public enum WaitForUrlComparisons
	{
		Equals,
		StartsWith
	}
}
