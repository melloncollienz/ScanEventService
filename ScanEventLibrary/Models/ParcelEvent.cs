using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary.Models
{
	public class ParcelEvent
	{
		public int EventId { get; set; }
		public int ParcelId { get; set; }
		public string Type { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string StatusCode { get; set; }
		public Device Device { get; set; }
		public User User { get; set; }

		//ideally this should be an extension method
		public static ScanEventData ToScanEventData(ParcelEvent parcelEvent)
		{
			return new ScanEventData()
			{
				EventId = parcelEvent.EventId,
				ParcelId = parcelEvent.ParcelId,
				CreatedDateTimeUtc = parcelEvent.CreatedDateTimeUtc,
				RunId = parcelEvent.User.RunId,
				StatusCode = parcelEvent.StatusCode,
				EventType = parcelEvent.Type
			};
		}
	}
}
