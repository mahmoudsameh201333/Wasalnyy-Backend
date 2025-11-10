using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Response
{
	public class AuthResult
	{
		public bool Success { get; set; }   
		public string Message { get; set; }       
		public string? Token { get; set; }
	}
}
