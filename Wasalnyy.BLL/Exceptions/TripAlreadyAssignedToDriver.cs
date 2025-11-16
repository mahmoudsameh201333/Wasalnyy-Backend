using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class TripAlreadyAssignedToDriver : Exception
    {
        public TripAlreadyAssignedToDriver(string? message) : base(message)
        {
        }
    }
}
