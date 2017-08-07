using System;

namespace ServerStatus.Models
{
	/// <summary>
	/// status of a contiuum pipeline instance
	/// </summary>
	public class ContinuumStatus
	{
		/// <summary>
		/// Map a string from continuum status to the enum
		/// </summary>
		/// <param name="severity"></param>
		/// <returns></returns>
		public static CtmSeverity MapSeverity( string severity )
		{
			CtmSeverity result;
			if (Enum.TryParse<CtmSeverity>(severity, out result))
				return result;
			else
				return CtmSeverity.notRunYet;
		}

		/// <summary>
		/// Severity of the continuum message
		/// </summary>
		/// <remarks>these name match string from continuum so we can use Enum.Parse</remarks>
		public enum CtmSeverity
		{
			/// <summary>
			/// The item is staged to run
			/// </summary>
			staged,
			/// <summary>
			/// The item is running
			/// </summary>
			processing,
			/// <summary>
			/// The item ran to completion without errors
			/// </summary>
			success,
			/// <summary>
			/// The item failed
			/// </summary>
			failure,
			/// <summary>
			/// The item was canceled programmitcally
			/// </summary>
			canceled,
			/// <summary>
			/// The status is unknown
			/// </summary>
			notRunYet,
			/// <summary>
			/// The item is waiting on a user 
			/// </summary>
			pending
		}

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="name"></param>
		/// <param name="url"></param>
		/// <param name="id"></param>
		public ContinuumStatus(CtmSeverity severity, string name = "", string url = "", string id = "")
		{
			Severity = severity;
			Name = name;
			Url = url;
			InstanceId = id;
		}

		/// <summary>
		/// the severity for the pipeline instance
		/// </summary>
		public CtmSeverity Severity { get; set; }
		/// <summary>
		/// the name of the pipeline instance
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The public Url for the instance into continuum
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// The instance id to pass to pipelineInstance
		/// </summary>
		public string InstanceId { get; set; }
	}
}