using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary.Models
{
	public class ResponseError
	{
        public string ErrorDescription { get; set; }
        public Exception Exception { get; set; }
    }


}
