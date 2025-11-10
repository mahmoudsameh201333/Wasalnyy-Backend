using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class OutOfZoneException : Exception
    {
        public OutOfZoneException(string message) : base(message)
        {
        }
    }
}
