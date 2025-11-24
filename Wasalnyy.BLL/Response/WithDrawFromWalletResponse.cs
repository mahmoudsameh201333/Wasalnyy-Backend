using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Response
{
    public class WithDrawFromWalletResponse
    {
       public bool IsSuccess { get; set; }
       public string Message { get; set; }
        public WithDrawFromWalletResponse(bool isSuccess, string message = "")
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
