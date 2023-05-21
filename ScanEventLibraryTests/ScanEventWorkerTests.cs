using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using ScanEventLibrary;
using ScanEventLibrary.ApiClient;
using ScanEventLibrary.DataSource;
using FluentAssertions;
using System;
using ScanEventLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanEventLibraryTests
{
	[TestClass]
	public class ScanEventWorkerTests
	{
		private Mock<IDataSource> _mockDataSource;
		private Mock<IScanEventClient> _mockScanEventClient;
		private Mock<ILogger> _mockLogger;

		private ScanEventWorker _worker;

        public ScanEventWorkerTests()
        {
			_mockDataSource = new Mock<IDataSource>();
			_mockScanEventClient = new Mock<IScanEventClient>();
			_mockLogger = new Mock<ILogger>();

			_worker = new ScanEventWorker(_mockLogger.Object, _mockScanEventClient.Object, _mockDataSource.Object);

        }

        [TestMethod]
		public void WhenOnStart_AndGetLastEventIdSuccessful_ThenLastEventIdCorrect()
		{

			//arrange
			var expected = 999;
			_mockDataSource.Setup(t => t.GetLastEventId()).Returns(expected);

			//act
			_worker.OnStart();

			//assert
			//this is a hack to test the new value, usually i wouldn't do this because I should be testing the return value of OnStart, but because 
			//it's a windows service it should manage itself
			PrivateObject po = new PrivateObject(_worker);
			var result = po.GetField("_lastEventId");
			result.Should().Be(expected);
			
		}

		[TestMethod]
		public void WhenOnStart_AndGetLastIdThrowsException_ThenHandleException()
		{
			//arrange
			_mockDataSource.Setup(t => t.GetLastEventId()).Throws(new Exception("Test Exception"));

			//act
			_worker.OnStart();

			//assert
			//todo: parameter checking
			_mockLogger.Verify(t => t.Info(It.IsAny<string>()), Times.Exactly(2));
			_mockLogger.Verify(t => t.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
		}

		[TestMethod]
		public async Task WhenEventTriggered_AndGetsScaneEventsCorrectly_ThenSaveEvents()
		{
			//arrange
			var eventId = 999;
			_mockDataSource.Setup(t => t.GetLastEventId()).Returns(eventId);

			var data = new ScanEventsResponse()
			{
				ScanEvents = new List<ParcelEvent>()
				{
					new ParcelEvent()
					{
						CreatedDateTimeUtc = DateTime.UtcNow,
						Device = new Device()
						{
							DeviceId = 1,
							DeviceTransactionId = 2
						},
						EventId = eventId + 1,
						ParcelId = 2,
						StatusCode = string.Empty,
						Type = "PICKUP",
						User = new User()
						{
							CarrierId = "NC",
							RunId = "TEST",
							UserId = "USER ID"
						}
					}
				}
			};

			var source = new ApiResponse<ScanEventsResponse>()
			{ 
				IsSuccessful = true ,
				Data = data	
			};

			var expected = new List<ScanEventData>()
			{ 
				ParcelEvent.ToScanEventData(data.ScanEvents.First())
			};
				
			_mockScanEventClient.Setup(t => t.GetScanEvents(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(source);
			_worker.OnStart();


			//act
			await _worker.EventTriggered();

			//assert
			_mockDataSource.Verify(t => t.SaveEvents(It.IsAny<List<ScanEventData>>()), Times.Once);

		}

		//I don't really feel like writing more unit tests but you get the idea
		//The general plan is to check the positive/negative scenarios, and to do as many verify calls as necessary
	}
}
