using ScanEventLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary.ApiClient
{
	public interface IScanEventClient
	{
		Task<ApiResponse<ScanEventsResponse>> GetScanEvents(int fromEvent, int limit);
	}
}
