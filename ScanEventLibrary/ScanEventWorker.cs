using NLog;
using ScanEventLibrary.ApiClient;
using ScanEventLibrary.DataSource;
using ScanEventLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanEventLibrary
{
	//if there were more service workers, make this an interface, 
	
	public class ScanEventWorker : IScanEventWorker
	{
		//storing the last event id like this is a bit argricultural, but it'll work for now, if it was web it'd have to be in some shared datasource
		//because of web/containers/request distribution between different servers etc.
        private int _lastEventId = -1;
		private ILogger _logger;
		private IScanEventClient _scanEventClient;
		private IDataSource _dataSource;
		private const int defaultEventLimit = 100;
        public ScanEventWorker(ILogger logger, IScanEventClient scanEventClient, IDataSource dataSource)
        {
			//todo: event log logging
			_logger = logger;
			_scanEventClient = scanEventClient;
			_dataSource = dataSource;
        }
        public void OnStart()
		{
			_logger.Info($"{nameof(OnStart)} Start");
			try
			{
				_lastEventId = _dataSource.GetLastEventId();
			}
			catch (Exception ex)
			{
				//todo: event log logging
				_logger.Error(ex, $"{nameof(ScanEventWorker)}.{nameof(OnStart)} threw exception");
			}
			_logger.Info($"{nameof(OnStart)} End");

		}

		public void OnStop()
		{
			_logger.Info($"{nameof(OnStop)} Start");

			try
			{
				if (_lastEventId != -1)
				{
					_dataSource.SaveLastEventId(_lastEventId);
				}
			}
			catch (Exception ex)
			{
				//todo: event log logging
				_logger.Error(ex, $"{nameof(ScanEventWorker)}.{nameof(OnStop)} threw exception");
			}
			_logger.Info($"{nameof(OnStop)} End");
		}

		public async Task EventTriggered()
		{
			_logger.Info($"{nameof(EventTriggered)} Start");
			try
			{
				var scanEvents = await _scanEventClient.GetScanEvents(_lastEventId, defaultEventLimit);
				if (!scanEvents.IsSuccessful)
				{
					_logger.Error($"Call to Get Scan Events failed with error");
					if (scanEvents.Errors.Count > 0)  //i don't have safe navigation operator in this version of .net :(
					{
						scanEvents.Errors.ForEach(t => _logger.Error(t.Exception, t.ErrorDescription));
					}
					throw new AggregateException(scanEvents.Errors.Select(t => t.Exception).ToList());
				}

				if (scanEvents.Data.ScanEvents.Count > 0)
				{
					var toSave = new List<ScanEventData>();
					scanEvents.Data.ScanEvents.ForEach(t => toSave.Add(ParcelEvent.ToScanEventData(t)));
					_dataSource.SaveEvents(toSave);
				}

			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"{nameof(ScanEventWorker)}.{nameof(EventTriggered)} failed with exception");
			}
			_logger.Info($"{nameof(EventTriggered)} End");
		}
			
		//this is a hack to get the value unit testing
		//i know there's another way to get the private values but this is a demo
		public int GetEventId()
		{
			return _lastEventId;
		}
	}
}