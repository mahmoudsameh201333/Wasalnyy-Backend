using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Response
{
    public class TransferWalletResponse
    {
        public bool IsSuccess;
        public string Message;

        public TransferWalletResponse(bool IsSuccess, string Message="")
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }
    }
}
