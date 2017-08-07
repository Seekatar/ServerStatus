using System;
using System.Collections.Generic;
using ServerStatus.Models;

namespace ServerStatus.Services
{
	/// <summary>
	/// Interface for getting server statuses
	/// </summary>
	public interface IStatusService
	{
		/// <summary>
		/// Get both Continuum and Zabbix status details
		/// </summary>
		/// <param name="count">count of items to return, defaults to 20, max 50</param>
		/// <returns>Last Update, Continuum, and Zabbix statuses</returns>
		(DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus( int count );

		/// <summary>
		/// Get Continuum status details
		/// </summary>
		/// <param name="count">count of items to return, defaults to 20, max 50</param>
		/// <returns>Last Update, Continuum statuses</returns>
		LastContinuumStatus GetContinuumStatus(int count);

		/// <summary>
		/// Get the details about a pipeline instance
		/// </summary>
		/// <param name="instanceId">instanceId of a pipeline</param>
		/// <returns>Pipeline status object</returns>
		PipelineStatus GetContinuumPipelineStatus(string instanceId);

		/// <summary>
		/// Get both Continuum and Zabbix statuses only
		/// </summary>
		/// <param name="count">count of items to return, defaults to 12, max 50</param>
		/// <returns>Continuum, and Zabbix statuses ids</returns>
		(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12);
	}

}