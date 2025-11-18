using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class AlreadyInTripException : Exception
    {
        public AlreadyInTripException(string? message) : base(message)
        {
        }
    }
}
