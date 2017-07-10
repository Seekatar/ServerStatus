using System;

namespace V1ServerStatus.Models
{
	public class ZabbixStatus
	{
		public ZabbixStatus(UInt16 priority, string name = "", string url = "")
		{
			Priority = priority;
			Name = name;
			Url = url;
		}

		public UInt16 Priority { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
	}
}