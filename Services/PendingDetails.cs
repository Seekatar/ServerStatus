using Newtonsoft.Json;

namespace ServerStatus.Services
{
	/// <summary>
	/// Details about a pending pipeline step
	/// </summary>
	public class PendingDetails
	{
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[JsonProperty(PropertyName ="title")]
		public string Title { get; set; }
		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>
		/// The question.
		/// </value>
		[JsonProperty(PropertyName ="question")]
		public string Question { get; set; }
		/// <summary>
		/// Gets or sets the output key used internall by continuum.
		/// </summary>
		/// <value>
		/// The output key.
		/// </value>
		[JsonProperty(PropertyName ="outputKey")]
		public string OutputKey { get; set; }
	}
}