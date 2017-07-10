using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using V1ServerStatus.Controllers;
using V1ServerStatus.Models;

namespace V1ServerStatus.Services
{
	public class ZabbixService
	{
		const int EVENT_CACHE_SIZE = 100;
		const int High = 5;
		private string _zabbixUrl;
		private string _zabbixLinkUrl;
		private string _zabbixPw;
		private string _zabbixUser;
		private string _zabbixSessionId;
		private ILogger _logger;

		struct trigger_struct
		{
			public Int32 triggerid;
			public UInt16 priority;
			public string description;
		};
		struct event_struct
		{
			public Int32 triggerid;
			public Int32 eventid;
			public bool recovered;
		};

		const int TRIGGER_CACHE_SIZE = 100;
		trigger_struct[] _triggers = new trigger_struct[TRIGGER_CACHE_SIZE];

		event_struct[] _events = new event_struct[EVENT_CACHE_SIZE];

		public ZabbixService(IConfiguration configuration, ILogger logger)
		{
			_zabbixUrl = configuration["Config:ZABBIX_PATH"];
			_zabbixPw = configuration["Config:ZABBIX_PW"];
			_zabbixUser = configuration["Config:ZABBIX_USER"];
			_zabbixLinkUrl = configuration["Config:ZABBIX_LINK_URL"];
			_logger = logger;
		}

		public IList<ZabbixStatus> StatusItems = new List<ZabbixStatus>();

		public void PollStatus(int count = 12)
		{
			StatusItems.Clear();

			try
			{
				getZabbixStatus(count, count);
			}
			catch (Exception e)
			{
				_logger.LogError( e, $"Error getting Zabbix data from {_zabbixUrl}");
				StatusItems.Add( new ZabbixStatus(High, $"Exception getting Zabbix data"));
				StatusItems.Add( new ZabbixStatus(High, e.Message));
			}
		}

		private void getZabbixStatus(int startIndex, int totalCount)
		{
			string body;
			WebClient wc;
			string result;

			if (_zabbixSessionId == null)
			{
				body = String.Format(@"{{
					""jsonrpc"": ""2.0"",
					""method"": ""user.login"",
					""params"": {{
						""user"": ""{0}"",
						""password"": ""{1}""
					}},
					""id"": 1,
					""auth"": null
				}}", _zabbixUser, _zabbixPw);

				wc = new WebClient();
				wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
				result = wc.UploadString(new Uri(_zabbixUrl), body);

				if (!String.IsNullOrEmpty(result))
				{
					dynamic root = JsonConvert.DeserializeObject(result);
					if (root != null)
					{
						_zabbixSessionId = root.result;
						_logger.LogInformation($"Login OK\nsessionId is {_zabbixSessionId}");
					}
					else
					{
						_logger.LogWarning($"Login failed: {result}");
					}
				}
			}

			// get status
			body = String.Format(@"{{
			""jsonrpc"": ""2.0"",
			""method"": ""event.get"",
			""params"": {{
					""output"": [""eventid"",""objectid"",""r_eventid""],
					""sortfield"":""clock"",
					""sortorder"":""DESC"",
					""value"":1,
					""limit"":{1}
			}},
			""id"": 3,
			""auth"": ""{0}""
			}}", _zabbixSessionId, totalCount);

			wc = new WebClient();
			wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			result = wc.UploadString(new Uri(_zabbixUrl), body);

			if (!String.IsNullOrEmpty(result))
			{
				dynamic root = JsonConvert.DeserializeObject(result);
				if (root != null)
				{
					string s = root.ToString();
					s = root.result.ToString();
					s = root.result.Count.ToString();
					_logger.LogInformation($"Event OK\nLength of events is {root.result.Count}");
					int i = 0;
					foreach (var o in root.result)
					{
						s = o.r_eventid.ToString();
						_logger.LogDebug(s);
						s = o.objectid.ToString();
						_logger.LogDebug(s);
						_events[i].triggerid = Int32.Parse(o.objectid.ToString());
						_events[i].eventid = Int32.Parse(o.eventid.ToString());
						_events[i].recovered = o.r_eventid.ToString() != "0";
						i++;
					}
					for (i = 0; i < totalCount && _events[i].eventid != 0; i++)
					{
						StatusItems.Add(mapZabbixStatus(_events[i]));
					}
					for (int j = i; j < totalCount; j++)
					{
						_events[startIndex + j].eventid = 0;
						StatusItems.Add(new ZabbixStatus(0));
					}

				}
			}
			else
			{
				_logger.LogWarning($"Event failed: {result}");
			}

		}
		ZabbixStatus mapZabbixStatus(event_struct events)
		{
			Int32 triggerId = events.triggerid;
			bool recovered = events.recovered;
			Int32 eventId = events.eventid;
			string description = string.Empty;
			UInt16 priority = 0;

			_logger.LogDebug($" TriggerId: {triggerId} recovered: {recovered}");

			// find it in triggers

			for (int i = 0; i < TRIGGER_CACHE_SIZE; i++)
			{
				if (_triggers[i].triggerid == triggerId)
				{
					priority = _triggers[i].priority;
					description = _triggers[i].description;
					_logger.LogDebug("Found trigger!");
					break;
				}
			}

			if (priority == 0)
			{
				// cache triggers
				var body = String.Format(@"{{
					""jsonrpc"": ""2.0"",
					""method"": ""trigger.get"",
					""params"": {{
					""output"":[""triggerid"",""priority"",""description""],
					""triggerids"":{0}
					}},
					""id"": 2,
					""auth"": ""{1}""
				}}", triggerId, _zabbixSessionId);
				_logger.LogDebug(body);

				var wc = new WebClient();
				wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
				var output = wc.UploadString(_zabbixUrl, body);
				if (!String.IsNullOrEmpty(output))
				{
					dynamic root = JsonConvert.DeserializeObject(output);
					if (root != null)
					{
						_logger.LogInformation($"Trigger OK\nLength of triggers is {root.result.Count}");
						for (int i = 0; i < root.result.Count; i++)
						{
							Int32 id = Int32.Parse(root.result[i].triggerid.ToString());

							// already have it?
							int j;
							for (j = 0; j < TRIGGER_CACHE_SIZE && _triggers[j].triggerid != 0; j++)
							{
								if (_triggers[j].triggerid == id)
								{
									_logger.LogDebug($"Already have trigger id {id}");
									priority = _triggers[j].priority;
									description = _triggers[j].description;
									break;
								}
							}
							if (_triggers[j].triggerid == 0)
							{
								_logger.LogDebug($"Added trigger {j} id of {id}");
								_triggers[j].triggerid = id;
								_triggers[j].priority = (root.result[i].priority);
								_triggers[j].description = (root.result[i].description);
								if (id == triggerId)
								{
									_logger.LogDebug("Matched newly added one");
									priority = _triggers[j].priority;
									description = _triggers[j].description;
								}
							}
						}
					}
					else
					{
						_logger.LogWarning($"Trigger failed: {output}");
					}
				}
			}

			var ret = new ZabbixStatus(recovered ? (UInt16)0 : priority, description, String.Format(_zabbixLinkUrl, triggerId, eventId));
			return ret;
		}
	}
}
