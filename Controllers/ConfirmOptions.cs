using Newtonsoft.Json;

namespace ServerStatus.Controllers
{
	/// <summary>
	/// class for passing in details about the confirm
	/// </summary>
	public class ConfirmOptions
	{
		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		[JsonProperty(PropertyName ="response")]
		public string Response { get; set; }
		/// <summary>
		/// Gets or sets the output key.
		/// </summary>
		/// <value>
		/// The output key.
		/// </value>
		[JsonProperty(PropertyName ="outputKey")]
		public string OutputKey { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ConfirmOptions"/> is confirm.
		/// </summary>
		/// <value>
		///   <c>true</c> if confirm; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty(PropertyName ="confirm")]
		public bool Confirm { get; set; }

	}
}