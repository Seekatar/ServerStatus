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
	}

		public PipelineStatus GetContinuumPipelineStatus(string instanceId)
		{
			// TODO 
			return new PipelineStatus();
		}

		public (DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus) GetContinuumStatus(int count)
		{
			if (DateTime.Now - _lastRetrieval > _retrievalInterval)
			{
				_continuum.PollStatus(MAX_COUNT);
				_lastRetrieval = DateTime.Now;
			}
			return (_lastRetrieval, _continuum.StatusItems.Take(count));
		}

		public (DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus(int count)
		{
			if (DateTime.Now - _lastRetrieval > _retrievalInterval)
			{
				_continuum.PollStatus(MAX_COUNT);
				_zabbix.PollStatus(MAX_COUNT);
				_lastRetrieval = DateTime.Now;
			}
			return (_lastRetrieval, _continuum.StatusItems.Take(count), _zabbix.StatusItems.Take(count));
		}

		public 	(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12)
		{
			(DateTime lastUpdate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) = GetStatus(count);
			return (ContinuumStatus.Select(o => o.Severity),ZabbixStatus.Select(o=>o.Priority));
		}

}

}