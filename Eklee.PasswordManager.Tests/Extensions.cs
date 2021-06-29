using Microsoft.Edge.SeleniumTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Eklee.PasswordManager.Tests
{
	public static class Extensions
	{
		public static void AssertTableColumnTextIsPresent(this EdgeDriver edge, string text)
		{
			var e = edge.FindElement(By.XPath($"//td[text() = '{text}']"));
			Assert.IsNotNull(e);
		}

		public static void AssertTableColumnTextIsMissing(this EdgeDriver edge, string text)
		{
			Assert.ThrowsException<NoSuchElementException>(() =>
			{
				var e = edge.FindElement(By.XPath($"//td[text() = '{text}']"));
			});
		}
	}
}
