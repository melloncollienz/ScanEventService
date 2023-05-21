using Microsoft.AspNetCore.WebUtilities;
using NLog;
using ScanEventLibrary.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanEventLibrary.ApiClient
{
	public class ScanEventClient : IScanEventClient
	{
		private static string scanEventUrl = "https://localhost:7117/v1/scans/scanevents";
        private ILogger _logger;
        private HttpClient _httpClient;

        public ScanEventClient(ILogger logger, HttpClient httpClient)
        {
            _logger = logger;  
            _httpClient = httpClient;
        }

		public async Task<ApiResponse<ScanEventsResponse>> GetScanEvents(int fromEvent = 1, int limit = 100)
		{
			var result = new ApiResponse<ScanEventsResponse>()
			{
				IsSuccessful = false
			};

			try
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "fromEvent", fromEvent.ToString() },
					{ "limit", limit.ToString() }
				};

				var response = await Get<ScanEventsResponse>(scanEventUrl, parameters);
				if (response != null)
				{
					result.IsSuccessful = true;
					result.Data = response;
				} 
				else
				{
					var error = new ResponseError()
					{
						ErrorDescription = "Response from api endpoint was null"
					};
					result.Errors.Add(error);
				}

			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"{nameof(GetScanEvents)} threw an exception");
				var error = new ResponseError()
				{
					ErrorDescription = "Api get threw exception",
					Exception = ex
				};
				result.Errors.Add(error);

			}
			return result;
		}

		private async Task<TResult> Get<TResult>(string uri, Dictionary<string, string> parameters)
		{
			try
			{
				if (string.IsNullOrEmpty(uri))
				{
					throw new ArgumentNullException("uri");
				}

				if (parameters != null)
				{
					uri = QueryHelpers.AddQueryString(uri, parameters);
				}
				var responseText = await _httpClient.GetStringAsync(uri); 
				var result = JsonSerializer.Deserialize<TResult>(responseText);
				return result;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, $"{nameof(Get)} threw an exception");
				throw;
			}
		}
	}
}
