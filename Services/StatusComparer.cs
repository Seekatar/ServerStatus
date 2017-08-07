using System.Collections.Generic;
using ServerStatus.Models;

namespace ServerStatus.Services
{
	internal class StatusComparer : IEqualityComparer<ContinuumStatus>
	{
		public bool Equals(ContinuumStatus x, ContinuumStatus y)
		{
			return string.Equals(x?.InstanceId, y?.InstanceId, System.StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(ContinuumStatus obj)
		{
			return obj.InstanceId.GetHashCode();
		}
	}
}