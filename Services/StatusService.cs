using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerStatus.Models;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Timers;

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
		private System.Timers.Timer _timer;

		public StatusService(IConfiguration configuration, ILogger<StatusService> logger)
		{
			_logger = logger;
			_continuum = new ContinuumService(configuration, logger);
			_zabbix = new ZabbixService(configuration, logger);
			int tempInt = 30;
			int.TryParse(configuration["Config:POLLING_INTERVAL_SEC"], out tempInt);
			_retrievalInterval = TimeSpan.FromSeconds(Math.Min(Math.Max(tempInt, 3), 120));

			_timer = new System.Timers.Timer(5000);
			_timer.Elapsed += poll;
			_timer.Start();

		}

		private void poll(object sender, ElapsedEventArgs e)
		{
			_timer.Enabled = false;
			_continuum.PollStatus(MAX_COUNT);
			if ( _continuum.UpdatedItems.Count > 0 )
			{
				sendUpdates(_continuum.UpdatedItems);
				_continuum.UpdatedItems.Clear();
			}
			_timer.Enabled = true;
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

		const int BUFFER_SIZE = 1000;
		IList<WebSocket> _sockets = new List<WebSocket>();

		public async Task SocketConnected(HttpContext context, WebSocket webSocket)
		{
			_sockets.Add(webSocket);
			await waitForClose(webSocket);
			return; 
		}

		private async Task waitForClose(WebSocket webSocket)
		{
			var rcvBuffer = new byte[BUFFER_SIZE];
			var rcv = await webSocket.ReceiveAsync(new ArraySegment<byte>(rcvBuffer), CancellationToken.None);

			while (webSocket.State == WebSocketState.Open)
			{
				if (!rcv.CloseStatus.HasValue)
					rcv = await webSocket.ReceiveAsync(new ArraySegment<byte>(rcvBuffer), CancellationToken.None);
			}
			await webSocket.CloseAsync(rcv.CloseStatus.Value, rcv.CloseStatusDescription, CancellationToken.None);
		}

		private void sendUpdates(IEnumerable<ContinuumStatus> updates)
		{
			if (updates.Count() > 0)
			{
				foreach (var webSocket in _sockets)
				{
					if (webSocket.State == WebSocketState.Open)
					{
						var buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(updates));
						webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
													 WebSocketMessageType.Text,
													 true, // eom
													 CancellationToken.None);
					}
				}
			}
		}
		
		public 	(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12)
		{
			(DateTime lastUpdate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) = GetStatus(count);
			return (ContinuumStatus.Select(o => o.Severity),ZabbixStatus.Select(o=>o.Priority));
		}

}

}