using System;

namespace Eklee.PasswordManager.Core
{
	public enum SecretItemEditMode
	{
		Default,
		Edit,
	}

	public class SecretItem
	{
		public SecretItem()
		{
			DivClass1 = "show-div";
			DivClass2 = "hide-div";
			InputId = Guid.NewGuid().ToString("N");
		}

		public string InputId { get; set; }

		public bool Enabled { get; set; }

		public bool Copied { get; set; }

		public bool ShowInput { get; set; }
		public string Value { get; set; }

		public string DivClass1 { get; set; }
		public string DivClass2 { get; set; }

		public SecretItemEditMode SwitchEditContext()
		{
			var i = DivClass2;
			DivClass2 = DivClass1;
			DivClass1 = i;

			if (DivClass2 == "show-div")
			{
				return SecretItemEditMode.Edit;
			}

			return SecretItemEditMode.Default;
		}

		public string DisplayName { get; set; }
		public string Name { get; set; }
	}
}
