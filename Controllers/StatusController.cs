using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServerStatus.Models;
using Microsoft.Extensions.Logging;
using System;
using ServerStatus.Services;
using System.ComponentModel.DataAnnotations;

namespace ServerStatus.Controllers
{
	/// <summary>
	/// controller for the API
	/// </summary>
	[Route("api/[controller]")]
	public class StatusController : Controller
	{
		private readonly IStatusService _service;
		private DateTime _lastRetrieval = DateTime.Now - TimeSpan.FromDays(1);
		private TimeSpan _retrievalInterval = TimeSpan.FromSeconds(10);

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="service"></param>
		/// <param name="logger"></param>
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
		public LastContinuumStatus GetContinuumStatus(int count = 12)
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
		/// <response code="200">Found the pipeline instance</response>
		/// <response code="404">If the instance id is not found</response>
		[HttpGet("pipelineInstance/{instanceId}")]
		[ProducesResponseType(404)]
		public IActionResult GetPipelineStatus(string instanceId)
		{
			var ret = _service.GetContinuumPipelineStatus(instanceId);
			if (ret != null)
				return Ok(ret);
			else
				return NotFound();
		}

		/// <summary>
		/// Confirms the pipeline step that is pending.
		/// </summary>
		/// <param name="instanceId">The instance identifier.</param>
		/// <param name="phase">The phase.</param>
		/// <param name="stage">The stage.</param>
		/// <param name="stepIndex">Index of the step.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		[HttpPost("pipelineInstance/{instanceId}/{phase}/{stage}/{stepIndex}")]
		public IActionResult ConfirmPipelineStep(string instanceId, string phase, string stage, int stepIndex, 
			[FromBody, Required] ConfirmOptions options )
												// [FromBody, Required] string response, [FromBody, Required] string outputKey, [FromBody, Required] bool confirm )
		{
			// confirm result = "success" or "failure"
			if (_service.ConfirmContinuumPipelineStep(instanceId, phase, stage, stepIndex, options.Response, options.OutputKey, options.Confirm))
				return Ok();
			else
				return NotFound();

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