using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary
{
	public interface IScanEventWorker
	{
		void OnStart();
		void OnStop();

		Task EventTriggered();

		int GetEventId(); //for unit testing
	}
}
