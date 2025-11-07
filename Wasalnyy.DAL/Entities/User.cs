using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.DAL.Entities
{
	public class User : IdentityUser
	{
		public string FullName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public int Age(){
			var today = DateTime.Today;
			int age = today.Year - DateOfBirth.Year;
			if (DateOfBirth.Date > today.AddYears(-age))
				age--;
			return age;
		}
		public string Image { get; set; }
		public string PhoneNumber { get; set; }
		public Gender Gender { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsDeleted { get; protected set; } = false;

	}
}
