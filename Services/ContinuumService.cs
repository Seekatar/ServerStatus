using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerStatus.Controllers;
using ServerStatus.Models;
using static ServerStatus.Models.ContinuumStatus;

namespace ServerStatus.Services
{
	public class ContinuumService
	{
		private string _ctmUrl;
		private string _ctmKey;
		private string _continuumLink;
		private ILogger _logger { get; set; }

		public ContinuumService(IConfiguration configuration, ILogger logger)
		{
			_ctmUrl = configuration["Config:CONTINUUM_PATH"];
			_ctmKey = configuration["Config:CONTINUUM_KEY"];
			_continuumLink = configuration["Config:CONTINUUM_LINK"];
			_logger = logger;
		}

		public IList<ContinuumStatus> StatusItems = new List<ContinuumStatus>();

		public void PollStatus(int count = 12)
		{
			StatusItems.Clear();

			try
			{
				getContinuumStatus(count);
			}
			catch (Exception e)
			{
				_logger.LogWarning(new EventId(), e, "yo");
			}
		}

		private void getContinuumStatus(int totalCount)
		{
			try
			{
				var result = new WebClient().DownloadString(new Uri(String.Format(_ctmUrl, _ctmKey, (DateTime.Now - TimeSpan.FromDays(3)).ToString("MM-dd-yyyy"))));

				var lines = result.Split("\r\n".ToCharArray());
				int count = 0;
				foreach (var line in lines)
				{
					var index = line.LastIndexOf('\t');
					var parts = line.Split("\t".ToCharArray());
					if (parts.Length == 6)
					{
						var last = parts.Last();
						var id = parts[0];
						var name = String.Join(" ", parts.Skip(2).Take(3));
						var url = String.Format(_continuumLink, parts[0]);
						if (last.Equals("success"))
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Ok, name, url, id));
						}
						else if (last.Equals("failure"))
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Failed, name, url, id));
						}
						else if (last.Equals("processing"))
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Running, name, url, id));
						}
						else if (last.Equals("staged"))
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Staged, name, url, id));
						}
						else if (last.Equals("canceled"))
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Canceled, name, url, id));
						}
						else
						{
							StatusItems.Add(new ContinuumStatus(CtmSeverity.Unknown));
						}
						count++;
					}
					if (count == totalCount)
						break;
				}
				for (int i = count; i < totalCount; i++)
				{
					StatusItems.Add(new ContinuumStatus(CtmSeverity.Unknown));
				}
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, "Error getting continuum data");
				StatusItems.Add( new ContinuumStatus(CtmSeverity.Failed,
					$"Exception getting Continuum data"));
				StatusItems.Add( new ContinuumStatus(CtmSeverity.Failed,
					e.Message));
			}
		}
	}
}
