using Newtonsoft.Json;

namespace ServerStatus.Services
{
	/// <summary>
	/// bucket of status info about a pipeline
	/// </summary>
	public class PipelineStatus
	{
		/// <summary>
		/// possible statuses of a pipeline, phase, stage, or step
		/// </summary>
		public enum ItemStatus
		{
			/// <summary>
			/// currently running
			/// </summary>
			processing,
			/// <summary>
			/// a step that was skipped due to conditional
			/// </summary>
			skipped,
			/// <summary>
			/// ran to completion without errors
			/// </summary>
			success,
			/// <summary>
			/// an error occurred
			/// </summary>
			failure,
			/// <summary>
			/// the item was canceled programmatically
			/// </summary>
			canceled,
			/// <summary>
			/// a step is waiting user intervention
			/// </summary>
			pending,
			/// <summary>
			/// a step hasn't had a chance to run yet
			/// </summary>
			notRunYet
		}
		/// <summary>
		/// the current phase if the instance is not complete
		/// </summary>
		[JsonProperty(PropertyName ="phase")]
		public string Phase { get; set; }
		/// <summary>
		/// the current stage if the instance is not complete
		/// </summary>
		[JsonProperty(PropertyName ="stage")]
		public string Stage { get; set; }
		/// <summary>
		/// the current step if the instance is not complete
		/// </summary>
		[JsonProperty(PropertyName ="step")]
		public string Step { get; set; }
		/// <summary>
		/// the step index if the instance is not complete
		/// </summary>
		[JsonProperty(PropertyName ="stepIndex")]
		public int StepIndex { get; set; }
		/// <summary>
		/// status of the item
		/// </summary>
		[JsonProperty(PropertyName ="status")]
		public ItemStatus Status { get; set; }
		/// <summary>
		/// Total numbers of steps that may run
		/// </summary>
		[JsonProperty(PropertyName ="totalSteps")]
		public int TotalSteps { get; internal set; }
		/// <summary>
		/// Total steps that ran 
		/// </summary>
		[JsonProperty(PropertyName ="totalSuccess")]
		public int TotalSuccess { get; internal set; }
		/// <summary>
		/// Total steps skipped due to conditional statements
		/// </summary>
		[JsonProperty(PropertyName ="totalSkipped")]
		public int TotalSkipped { get; internal set; }
		/// <summary>
		/// Total steps that failed
		/// </summary>
		[JsonProperty(PropertyName ="totalFailed")]
		public int TotalFailed { get; internal set; }
		/// <summary>
		/// Details if the pipeline is pending
		/// </summary>
		[JsonProperty(PropertyName ="pending")]
		public PendingDetails Pending { get; internal set; }
	}
}