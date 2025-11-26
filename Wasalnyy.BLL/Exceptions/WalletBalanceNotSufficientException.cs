using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class WalletBalanceNotSufficientException : Exception
    {
        public WalletBalanceNotSufficientException(string? message) : base(message)
        {
        }
    }
}
