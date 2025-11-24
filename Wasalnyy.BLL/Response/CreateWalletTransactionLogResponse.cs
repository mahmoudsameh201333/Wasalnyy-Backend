using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Response
{
    public class CreateWalletTransactionLogResponse
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public CreateWalletTransactionLogResponse(bool isSuccess, string message = "")
        {
            this.isSuccess = isSuccess;
            this.Message = message;
        }
    }
}
