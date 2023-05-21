using ScanEventLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ScanEventService
{
	public partial class Service1 : ServiceBase
	{
		private IScanEventWorker _worker;
		private Timer _timer;


		public Service1()
		{
			InitializeComponent();
			_timer = new Timer();
			_timer.Interval = 360000;
			_timer.Elapsed += _timer_Elapsed;
		}

		//There is an arguement that the time should live in the ScanEventLibrary
		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_worker.EventTriggered();
		}

		protected override void OnStart(string[] args)
		{
			_worker.OnStart();
			_timer.Start();
		}

		protected override void OnStop()
		{
			_worker.OnStop();
		}
	}
}
