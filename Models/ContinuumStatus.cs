using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

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
			/// The item was canceled programmatically
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
		/// <param name="severity">The severity.</param>
		/// <param name="name">The name.</param>
		/// <param name="url">The URL.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="pipelineName">Name of the pipeline.</param>
		/// <param name="group">The group.</param>
		/// <param name="project">The project.</param>
		public ContinuumStatus(CtmSeverity severity, string name = "", string url = "", string id = "", string pipelineName = "", string group = "", string project = "" )
		{
			Severity = severity;
			Name = name;
			Url = url;
			InstanceId = id;
			PipelineName = pipelineName;
			Group = group;
			Project = project;
		}

		/// <summary>
		/// the severity for the pipeline instance
		/// </summary>
		[JsonProperty(PropertyName = "severity")]
		public CtmSeverity Severity { get; set; }
		/// <summary>
		/// the name of the pipeline instance
		/// </summary>
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
		/// <summary>
		/// The public Url for the instance into continuum
		/// </summary>
		[JsonProperty(PropertyName = "url")]
		public string Url { get; set; }
		/// <summary>
		/// The instance id to pass to pipelineInstance
		/// </summary>
		[JsonProperty(PropertyName = "instanceId")]
		public string InstanceId { get; set; }
		/// <summary>
		/// Gets the name of the pipeline.
		/// </summary>
		/// <value>
		/// The name of the pipeline.
		/// </value>
		[JsonProperty(PropertyName = "pipelineName")]
		public string PipelineName { get; private set; }
		/// <summary>
		/// Gets the group.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
 		[JsonProperty(PropertyName = "group")]

		public string Group { get; private set; }
		/// <summary>
		/// Gets the project.
		/// </summary>
		/// <value>
		/// The project.
		/// </value>
		[JsonProperty(PropertyName = "project")]
		public string Project { get; private set; }
	}
}