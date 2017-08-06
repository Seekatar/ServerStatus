namespace ServerStatus.Models
{
	public class ContinuumStatus
	{
		public enum CtmSeverity
		{
			Staged,
			Running,
			Ok,
			Failed,
			Canceled,
			Unknown
		}
		public ContinuumStatus(CtmSeverity severity, string name = "", string url = "", string id = "")
		{
			Severity = severity;
			Name = name;
			Url = url;
			InstanceId = id;
		}

		public CtmSeverity Severity { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string InstanceId { get; set; }
	}
}