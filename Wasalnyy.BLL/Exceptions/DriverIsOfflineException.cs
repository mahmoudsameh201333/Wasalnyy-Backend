using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class DriverIsOfflineException : Exception
    {
        public DriverIsOfflineException(string? message) : base(message)
        {
        }
    }
}
