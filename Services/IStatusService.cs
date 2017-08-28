using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
		(IEnumerable<ContinuumStatus.PipelineStatus> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12);

		/// <summary>
		/// Get both Continuum statuses only
		/// </summary>
		/// <param name="count">count of items to return, defaults to 12, max 50</param>
		/// <returns>Continuum statuses ids</returns>
		IEnumerable<Tuple<ContinuumStatus.PipelineStatus, string>> ContinuumStatusOnly(int count = 12);

		/// <summary>
		/// called when a websocket client attaches
		/// </summary>
		/// <param name="context"></param>
		/// <param name="webSocket"></param>
		/// <returns></returns>
		Task SocketConnected(HttpContext context, WebSocket webSocket);

		/// <summary>
		/// Confirms the pipeline step.
		/// </summary>
		/// <param name="instanceId">The instance identifier.</param>
		/// <param name="phase">The phase.</param>
		/// <param name="stage">The stage.</param>
		/// <param name="stepIndex">Index of the step.</param>
		/// <param name="response">The response.</param>
		/// <param name="outputKey">The output key.</param>
		/// <param name="confirm">if set to <c>true</c> [confirm].</param>
		/// <returns></returns>
		bool ConfirmContinuumPipelineStep(string instanceId, string phase, string stage, int stepIndex, string response, string outputKey, bool confirm);
	}

}