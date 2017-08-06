using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServerStatus.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using ServerStatus.Services;

namespace ServerStatus.Controllers
{
	[Route("api/[controller]")]
	public class StatusController : Controller
	{
		private readonly IStatusService _service;
		private DateTime _lastRetrieval = DateTime.Now - TimeSpan.FromDays(1);
		private TimeSpan _retrievalInterval = TimeSpan.FromSeconds(10);

		public StatusController(IStatusService service, ILogger<StatusController> logger)
		{
			_service = service;
		}

		/// <summary>
		/// Get both Continuum and Zabbix status details
		/// </summary>
		/// <param name="count">count of items to return, defaults to 12, max 50</param>
		/// <returns>Last Update, Continuum, and Zabbix statuses</returns>
		[HttpGet("continuum")]
		public (DateTime LastRetrieval, IEnumerable<ContinuumStatus> ContinuumStatus) GetContinuumStatus(int count = 12)
		{
			return _service.GetContinuumStatus(count);
		}

		/// <summary>
		/// Get Continuum status details
		/// </summary>
		/// <param name="count">count of items to return, defaults to 20, max 50</param>
		/// <returns>Last Update, Continuum statuses</returns>
		[HttpGet]
		public (DateTime LastRetrieval, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus(int count = 12)
		{
			return _service.GetStatus(count);
		}

		/// <summary>
		/// Get the details about a pipeline instance
		/// </summary>
		/// <param name="instanceId">instanceId of a pipeline</param>
		/// <returns>Pipeline status object</returns>
		[HttpGet("pipelineInstance")]
		public PipelineStatus GetContinuumPipelineStatus(string instanceId)
		{
			return _service.GetContinuumPipelineStatus(instanceId);
		}

		/// <summary>
		/// Get both Continuum and Zabbix statuses only
		/// </summary>
		/// <param name="count">count of items to return, defaults to 12, max 50</param>
		/// <returns>Continuum, and Zabbix statuses ids</returns>
		[HttpGet("statusOnly")] // http://localhost:5000/api/status/statusOnly
		public (IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12)
		{
			return _service.StatusOnly(count);
		}

	}
}