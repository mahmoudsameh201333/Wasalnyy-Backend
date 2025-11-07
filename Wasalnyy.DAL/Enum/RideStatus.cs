using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Enum
{
	public enum RideStatus
	{
		Pending = 0,
		Accepted = 1,
		Rejected = 2,
		Cancelled = 3,
		DriverWaiting = 4,
		InProgress = 5,
		Completed = 6
	}
}
