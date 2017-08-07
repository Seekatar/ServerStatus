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
		public string Phase { get; set; }
		/// <summary>
		/// the current stage if the instance is not complete
		/// </summary>
		public string Stages { get; set; }
		/// <summary>
		/// the current step if the instance is not complete
		/// </summary>
		public string Step { get; set; }
		/// <summary>
		/// the step index if the instance is not complete
		/// </summary>
		public int StepIndex { get; set; }
		/// <summary>
		/// status of the item
		/// </summary>
		public ItemStatus Status { get; set; }
		/// <summary>
		/// Total numbers of steps that may run
		/// </summary>
		public int TotalSteps { get; internal set; }
		/// <summary>
		/// Total steps that ran 
		/// </summary>
		public int TotalSuccess { get; internal set; }
		/// <summary>
		/// Total steps skipped due to conditional statements
		/// </summary>
		public int TotalSkipped { get; internal set; }
		/// <summary>
		/// Total steps that failed
		/// </summary>
		public int TotalFailed { get; internal set; }
	}
}