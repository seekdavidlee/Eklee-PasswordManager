using System;
using System.Diagnostics;

namespace Eklee.PasswordManager.Tests
{
	public static class TestLogger
	{
		public static void WriteLog(string message)
		{
			Trace.WriteLine($"{DateTime.UtcNow} {message}");
		}
	}
}
