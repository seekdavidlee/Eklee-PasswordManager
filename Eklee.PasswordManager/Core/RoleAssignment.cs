using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eklee.PasswordManager.Core
{
	public class RoleAssignmentResult
	{
		[JsonProperty("value")]
		public List<RoleAssignment> Values { get; set; }
	}

	public class Role
	{
		public string RoleDefinitionId { get; set; }
	}

	public class RoleAssignment
	{
		public string Id { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }

		[JsonProperty("properties")]
		public Role Role { get; set; }
	}
}
