using System.Collections.Generic;
using ServerStatus.Models;

namespace ServerStatus.Services
{
	internal class StatusStateComparer : IEqualityComparer<ContinuumStatus>
	{
		public bool Equals(ContinuumStatus x, ContinuumStatus y)
		{
			return string.Equals(x?.InstanceId, y?.InstanceId, System.StringComparison.OrdinalIgnoreCase) && x.Severity == y.Severity;
		}

		public int GetHashCode(ContinuumStatus obj)
		{
			return (obj.InstanceId+obj.Severity.ToString()).GetHashCode();
		}
	}
}