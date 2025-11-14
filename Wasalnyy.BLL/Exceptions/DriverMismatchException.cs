using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class DriverMismatchException : Exception
    {
        public DriverMismatchException(string message) : base(message)
        {
        }
    }
}
