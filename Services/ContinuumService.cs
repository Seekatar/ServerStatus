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
using static System.Diagnostics.Debug;

namespace ServerStatus.Services
{
	class ContinuumService
	{
		private string _ctmUrl;
		private string _ctmKey;
		private string _continuumLink;
		private ILogger _logger { get; set; }

		public ContinuumService(IConfiguration configuration, ILogger logger)
		{
			_ctmUrl = configuration["Config:CONTINUUM_API"];
			_ctmKey = configuration["Config:CONTINUUM_KEY"];
			_continuumLink = configuration["Config:CONTINUUM_LINK"];
			_logger = logger;
		}

		public IList<ContinuumStatus> StatusItems = new List<ContinuumStatus>();

		internal PipelineStatus GetPipelineStatus(string instanceId)
		{
			var wc = new WebClient();
			wc.Headers["Authorization"] = $"token {_ctmKey}";
			var result = wc.DownloadString(new Uri(_ctmUrl + $"get_pipelineinstance?pi={instanceId}&include_stages=True"));

			dynamic root = JsonConvert.DeserializeObject(result);

			var piStatus = new PipelineStatus();

			foreach (var p in root.Response.phases)
			{
				foreach (var s in p.stages)
				{
					dynamic ss = JsonConvert.DeserializeObject(s.ToString().Substring(s.ToString().IndexOf("{")));
					var stepNo = -1;
					foreach (var step in ss.steps)
					{
						stepNo++;
						if (step.when == "never")
							continue;

						var msg = step.name.ToString();
						if (string.IsNullOrWhiteSpace(msg))
							msg = step.plugin.label;
						var status = step.status?.ToString();
						if (string.IsNullOrWhiteSpace(status))
							status = "not run";

						WriteLine($"        {status} -> {msg}");
						piStatus.TotalSteps++;

						if (step.status != null)
						{
							switch (step.status.ToString())
							{
								case "success":
									piStatus.TotalSuccess++;
									break;
								case "skipped":
									piStatus.TotalSkipped++;
									break;
								case "processing":
								case "failure":
								case "canceled":
									piStatus.TotalFailed++;
									piStatus.StepIndex = piStatus.TotalSteps;
									piStatus.Phase = p.name;
									piStatus.Stages = ss.name;
									piStatus.Step = msg;
									break;
								case "pending":
									piStatus.Status = PipelineStatus.ItemStatus.pending;
									piStatus.StepIndex = piStatus.TotalSteps;
									piStatus.Phase = p.name;
									piStatus.Stages = ss.name;
									piStatus.Step = msg;
									break;
								default:
									break; // not run yet
							}
						}
					}

				}

			}

			//WriteLine($"Total {total} Success {totalSuccess} Failed {totalFailed} Index {runningIndex} Skipped {totalSkipped} Complete {100 * runningIndex / total}%");
			//WriteLine($"Pending step is {pendingStepNo}");
			//WriteLine($"Status is {piStatus} at '{currentPhase}'->'{currentStage}'->'{currentStep}'");
			return piStatus;
		}

		public void PollStatus(int count = 12)
		{
			try
			{
				getContinuumStatus(count);
			}
			catch (Exception e)
			{
				_logger.LogWarning(new EventId(), e, "yo");
			}
		}

		private void getContinuumStatus(int maxItems)
		{
			var statusItems = new List<ContinuumStatus>();
			var updatedItems = new List<ContinuumStatus>();

			try
			{
				var wc = new WebClient();
				wc.Headers["Authorization"] = $"token {_ctmKey}";
				var result = wc.DownloadString(new Uri(_ctmUrl + $"list_pipelineinstances?&since={(DateTime.Now - TimeSpan.FromDays(3)).ToString("MM - dd - yyyy")}&output_format=text&header=false"));

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
							statusItems.Add(new ContinuumStatus(CtmSeverity.success, name, url, id));
						}
						else if (last.Equals("failure"))
						{
							statusItems.Add(new ContinuumStatus(CtmSeverity.failure, name, url, id));
						}
						else if (last.Equals("processing"))
						{
							statusItems.Add(new ContinuumStatus(CtmSeverity.processing, name, url, id));
						}
						else if (last.Equals("staged"))
						{
							statusItems.Add(new ContinuumStatus(CtmSeverity.staged, name, url, id));
						}
						else if (last.Equals("canceled"))
						{
							statusItems.Add(new ContinuumStatus(CtmSeverity.canceled, name, url, id));
						}
						else
						{
							statusItems.Add(new ContinuumStatus(CtmSeverity.notRunYet));
						}
						count++;
					}
					if (count == maxItems)
						break;
				}
				updatedItems = statusItems.Except(StatusItems, new StatusComparer() ).ToList();
				updatedItems.AddRange(statusItems.Where( o => StatusItems.Any( o2 => o2.InstanceId == o.InstanceId && o2.Severity != o.Severity) ));
				if ( updatedItems.Count > 0 )
				{
					foreach ( var i in updatedItems )
					{
						WriteLine($"Updated pipeline {i.InstanceId} {i.Name} ");
					}
				}
				StatusItems = statusItems;
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, "Error getting continuum data");
				StatusItems.Add( new ContinuumStatus(CtmSeverity.failure,
					$"Exception getting Continuum data"));
				StatusItems.Add( new ContinuumStatus(CtmSeverity.failure,
					e.Message));
			}
		}
	}
}
