using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eklee.PasswordManager.Core
{
	public class SecretItemList
	{
		[JsonProperty("value")]
		public List<SecretItem> Values { get; set; }
	}

	public class SecretValue
	{
		public string Value { get; set; }
	}

	public class SecretItem
	{
		public string Id { get; set; }
		public bool Enabled { get; set; }

		public bool Copied { get; set; }

		public bool ShowInput { get; set; }
		public string Value { get; set; }

		public string Name
		{
			get
			{
				var parts = Id.Split('/');
				return parts[parts.Length - 1];
			}
		}
	}
}
