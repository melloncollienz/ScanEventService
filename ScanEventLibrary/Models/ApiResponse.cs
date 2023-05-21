using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary.Models
{
	public class ApiResponse<TResponse>
	{

        public bool IsSuccessful { get; set; }

        public TResponse Data { get; set; }

		public List<ResponseError> Errors { get; set; } = new List<ResponseError>(); 

    }
}
