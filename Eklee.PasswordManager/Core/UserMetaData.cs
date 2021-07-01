using System;
using System.Collections.Generic;

namespace Eklee.PasswordManager.Core
{
	public class UserMetaData
	{
		public DateTime? Created { get; set; }
		public List<SecretMetaData> Items { get; set; }
	}
}
