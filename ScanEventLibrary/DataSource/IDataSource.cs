using ScanEventLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary.DataSource
{
	public interface IDataSource
	{
		int GetLastEventId();
		void SaveLastEventId(int eventId);
		void SaveEvents(List<ScanEventData> events);
	}
}
