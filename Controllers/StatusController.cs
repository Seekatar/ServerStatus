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

		[HttpGet]
		public (DateTime LastRetrieval, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) Get(int count = 12)
		{
			return _service.GetStatus(count);
		}

		[HttpGet("statusOnly")] // http://localhost:5000/api/status/statusOnly
		public (IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12)
		{
			return _service.StatusOnly(count);
		}

	}
}