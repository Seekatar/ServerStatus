namespace V1ServerStatus.Models
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
		public ContinuumStatus(CtmSeverity severity, string name = "", string url = "")
		{
			Severity = severity;
			Name = name;
			Url = url;
		}

		public CtmSeverity Severity { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
	}
}