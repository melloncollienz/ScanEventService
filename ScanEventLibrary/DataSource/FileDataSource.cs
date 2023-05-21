using NLog;
using ScanEventLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanEventLibrary.DataSource
{
	public class FileDataSource : IDataSource
	{
		private string saveFileName = "scanevents.json";
		private string lastEventFileName = "lastEvent.txt";

		private ILogger _logger;


		//I'm still keeping this as a file based datastore because i don't wanna add a docker container
		//but i'd add a new data store to whatever data access or orm you have
		//And depending on how you are access the datastore, have another interface to handle the actual getting/saving data.
        public FileDataSource(ILogger logger)
        {
            _logger = logger;   
        }

		public void SaveLastEventId(int eventId)
		{
			try
			{
				File.WriteAllText(lastEventFileName, eventId.ToString());
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				throw;
			}

		}

		public int GetLastEventId()
		{
			try
			{
				var result = 0;
				if (File.Exists(lastEventFileName))
				{


					var eventId = File.ReadAllText(lastEventFileName);
					if (!int.TryParse(eventId, out result))
					{
						result = 0;
					}
				}
				return result;

			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				throw;
			}
		}

		//and again there will be some extra logic in here for add/update depending on what data is coming in and what's already in the db.
		public void SaveEvents(List<ScanEventData> events)
		{
			try
			{
				var currentData = new List<ScanEventData>();
				if (File.Exists(saveFileName))
				{
					var content = File.ReadAllText(saveFileName);
					if (!string.IsNullOrEmpty(content))
					{
						var deserialized = JsonSerializer.Deserialize<List<ScanEventData>>(content);
						if (deserialized != null)
						{
							currentData.AddRange(deserialized);
						}

					}
				}

				currentData.AddRange(events);
				var serialized = JsonSerializer.Serialize(currentData);
				File.WriteAllText(saveFileName, serialized);


			}
			catch (Exception e)
			{
				_logger.Error(e);
				throw;
			}
		}


		public List<ScanEventData> GetEvents()
		{
			try
			{
				if (!File.Exists(saveFileName))
				{
					return new List<ScanEventData>();
				}

				var content = File.ReadAllText(saveFileName);
				return JsonSerializer.Deserialize<List<ScanEventData>>(content) ?? new List<ScanEventData>();


			}
			catch (Exception e)
			{
				_logger.Error(e);
				throw;
			}
		}
	}
}
