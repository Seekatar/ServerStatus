using System;

namespace ServerStatus.Models
{
	/// <summary>
	/// Zabbix status object
	/// </summary>
	public class ZabbixStatus
	{
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="name"></param>
		/// <param name="url"></param>
		public ZabbixStatus(UInt16 priority, string name = "", string url = "")
		{
			Priority = priority;
			Name = name;
			Url = url;
		}

		/// <summary>
		/// prority of the item
		/// </summary>
		public UInt16 Priority { get; set; }
		/// <summary>
		/// name 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// URL to Zabbix error
		/// </summary>
		public string Url { get; set; }
	}
}