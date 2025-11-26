using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class WithdrawException : Exception
    {
        public WithdrawException(string? message) : base(message)
        {
        }
    }
}
