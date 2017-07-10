using System;
using System.Collections.Generic;
using V1ServerStatus.Models;

namespace V1ServerStatus.Services
{
public interface IStatusService
{
	(DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus( int count );
	(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12);
}

}