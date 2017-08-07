using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerStatus.Models;

namespace ServerStatus.Services
{
class StatusService : IStatusService
{
	const int MAX_COUNT = 40;
	private ILogger<StatusService> _logger;
	private ContinuumService _continuum;
	private ZabbixService _zabbix;
	private DateTime _lastRetrieval = DateTime.Now - TimeSpan.FromDays(1);
	private TimeSpan _retrievalInterval = TimeSpan.FromSeconds(30);

	public StatusService(IConfiguration configuration, ILogger<StatusService> logger)
	{
		_logger = logger;
		_continuum = new ContinuumService(configuration,logger);
		_zabbix = new ZabbixService(configuration,logger);
		int tempInt = 30;
		int.TryParse(configuration["Config:POLLING_INTERVAL_SEC"], out tempInt);
		_retrievalInterval = TimeSpan.FromSeconds(Math.Min(Math.Max(tempInt,3),120));
	}

		public PipelineStatus GetContinuumPipelineStatus(string instanceId)
		{
			return _continuum.GetPipelineStatus(instanceId);
		}

		public LastContinuumStatus GetContinuumStatus(int count)
		{
			if (DateTime.Now - _lastRetrieval > _retrievalInterval)
			{
				_continuum.PollStatus(MAX_COUNT);
				_lastRetrieval = DateTime.Now;
			}
			return new LastContinuumStatus(_lastRetrieval, _continuum.StatusItems.Take(count));
		}

		public (DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus(int maxItems)
		{
			if (DateTime.Now - _lastRetrieval > _retrievalInterval)
			{
				_continuum.PollStatus(MAX_COUNT);
				_zabbix.PollStatus(MAX_COUNT);
				_lastRetrieval = DateTime.Now;
			}
			return (_lastRetrieval, _continuum.StatusItems.Take(maxItems), _zabbix.StatusItems.Take(maxItems));
		}

		public 	(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12)
		{
			(DateTime lastUpdate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) = GetStatus(count);
			return (ContinuumStatus.Select(o => o.Severity),ZabbixStatus.Select(o=>o.Priority));
		}

}

}