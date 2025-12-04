using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Account
{
	public class FacebookLoginDto
	{
		public string AccessToken { get; set; }
	}

	public class FacebookUserInfoDto
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
	}
}
