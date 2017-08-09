using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerStatus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
		public List<ContinuumStatus> _statusItems = new List<ContinuumStatus>();
		public List<ContinuumStatus> _updatedItems = new List<ContinuumStatus>();

		public ContinuumService(IConfiguration configuration, ILogger logger)
		{
			_ctmUrl = configuration["Config:CONTINUUM_API"];
			_ctmKey = configuration["Config:CONTINUUM_KEY"];
			_continuumLink = configuration["Config:CONTINUUM_LINK"];
			_logger = logger;
		}

		public IEnumerable<ContinuumStatus> StatusItems => _statusItems;
		public IEnumerable<ContinuumStatus> GetUpdates()
		{
			List<ContinuumStatus> ret;
			lock (_updatedItems)
			{
				ret = new List<ContinuumStatus>(_updatedItems);
				_updatedItems.Clear();
			}
			return ret;
		}

		internal PipelineStatus GetPipelineStatus(string instanceId)
		{
			var wc = new WebClient();
			wc.Headers["Authorization"] = $"token {_ctmKey}";
			var result = wc.DownloadString(new Uri(_ctmUrl + $"get_pipelineinstance?pi={instanceId}&include_stages=True"));

			if (!result.StartsWith("{") && result.IndexOf("not found") > 0)
				return null;

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
									piStatus.Stage = ss.name;
									piStatus.Step = msg;
									break;
								case "pending":
									piStatus.Status = PipelineStatus.ItemStatus.pending;
									piStatus.StepIndex = piStatus.TotalSteps;
									piStatus.Phase = p.name;
									piStatus.Stage = ss.name;
									piStatus.Step = msg;
									piStatus.Pending = new PendingDetails()
									{
										OutputKey = step.plugin.args.result_key,
										Question = step.plugin.args.text,
										Title = step.plugin.args.title
									};
									break;
								default:
									break; // not run yet
							}
						}
					}

				}

			}
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

			var uri = _ctmUrl + $"list_pipelineinstances?&since={(DateTime.Now - TimeSpan.FromDays(3)).ToString("MM-dd-yyyy")}&output_format=text&header=false";
			try
			{
				var wc = new WebClient();
				wc.Headers["Authorization"] = $"token {_ctmKey}";
				var result = wc.DownloadString(new Uri(uri));

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
						var url = String.Format(_continuumLink, parts[0]);
						var status = new ContinuumStatus(CtmSeverity.processing, parts[1], url, id, parts[2], parts[3], parts[4]);
						if (last.Equals("success"))
						{
							status.Severity = CtmSeverity.success;
						}
						else if (last.Equals("failure"))
						{
							status.Severity = CtmSeverity.failure;
						}
						else if (last.Equals("processing"))
						{
							status.Severity = isPending(id) ? CtmSeverity.pending : CtmSeverity.processing;
						}
						else if (last.Equals("staged"))
						{
							status.Severity = CtmSeverity.staged;
						}
						else if (last.Equals("canceled"))
						{
							status.Severity = CtmSeverity.canceled;
						}
						else
						{
							status.Severity = CtmSeverity.notRunYet;
						}
						statusItems.Add(status);
						count++;
					}
					if (count == maxItems)
						break;
				}
				lock (_updatedItems)
				{
					_updatedItems = statusItems.Except(StatusItems, new StatusComparer()).ToList();
					_updatedItems.AddRange(statusItems.Where(o => _statusItems.Any(o2 => o2.InstanceId == o.InstanceId && o2.Severity != o.Severity)));
					if (_updatedItems.Count > 0)
					{
						foreach (var i in _updatedItems)
						{
							WriteLine($"Updated pipeline {i.InstanceId} {i.Name} ");
						}
					}
				}
				_statusItems = statusItems;
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, $"Error getting continuum data from {uri}");
				_statusItems.Add(new ContinuumStatus(CtmSeverity.failure,
					$"Exception getting Continuum data from {uri}"));
				_statusItems.Add(new ContinuumStatus(CtmSeverity.failure,
					e.Message));
			}
		}

		private bool isPending(string id)
		{
			return (GetPipelineStatus(id)?.Status ?? PipelineStatus.ItemStatus.failure ) == PipelineStatus.ItemStatus.pending;
		}

		internal bool ConfirmContinuumPipelineStep(string instanceId, string phase, string stage, int stepIndex, string response, string outputKey, bool confirm)
		{
			var statusItems = new List<ContinuumStatus>();

			var status = confirm ? "success" : "failure";
			var uri = _ctmUrl + $"update_pi_step_status?pi={instanceId}&phase={phase}&stage={stage}&step={stepIndex}&detail={response}&result_key={outputKey}&status={status}";
			try
			{
				var wc = new WebClient();
				wc.Headers["Authorization"] = $"token {_ctmKey}";
				var result = wc.DownloadString(new Uri(uri));
				/*{
						"ErrorCode": "",
						"ErrorDetail": "",
						"ErrorMessage": "",
						"Method": "update_pi_step_status",
						"Response": {
							"result": "Pipeline instance step updated"
						}
					}
				*/
				dynamic returnMsg = JsonConvert.DeserializeObject(result);
				if (string.IsNullOrEmpty(returnMsg.ErrorCode.ToString()) && returnMsg.Response.result.ToString().IndexOf("updated") > 0)
					return true;
				else
					throw new Exception($"Error updating step: {returnMsg.ErrorMessage}({returnMsg.ErrorCode}) {returnMsg.ErrorDetail}");
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, $"Error getting continuum data from {uri}");
				_statusItems.Add(new ContinuumStatus(CtmSeverity.failure,
					$"Exception getting Continuum data from {uri}"));
				_statusItems.Add(new ContinuumStatus(CtmSeverity.failure,
					e.Message));

				return false;
			}
		}
	}
}
