using ServerStatus.Models;
using System;
using System.Collections.Generic;

namespace ServerStatus.Services
{
	/// <summary>
	/// return payload for getting continuum status
	/// </summary>
	public class LastContinuumStatus
	{
		/// <summary>
		/// Collection of status objects
		/// </summary>
		public IEnumerable<ContinuumStatus> ContinuumStatus;
		/// <summary>
		/// Last time the status collection was updated
		/// </summary>
		public DateTime LastRetrieval;

		/// <summary>
		/// constructor 
		/// </summary>
		/// <param name="lastRetrieval"></param>
		/// <param name="continuumStatus"></param>
		public LastContinuumStatus(DateTime lastRetrieval, IEnumerable<ContinuumStatus> continuumStatus)
		{
			this.LastRetrieval = lastRetrieval;
			ContinuumStatus = continuumStatus;
		}
	}
}