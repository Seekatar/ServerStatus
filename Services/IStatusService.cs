using System;
using System.Collections.Generic;
using ServerStatus.Models;

namespace ServerStatus.Services
{
public interface IStatusService
{
	(DateTime LastUpate, IEnumerable<ContinuumStatus> ContinuumStatus, IEnumerable<ZabbixStatus> ZabbixStatus) GetStatus( int count );
	(IEnumerable<ContinuumStatus.CtmSeverity> ContinuumStatus, IEnumerable<UInt16> ZabbixStatus) StatusOnly(int count = 12);
}

}