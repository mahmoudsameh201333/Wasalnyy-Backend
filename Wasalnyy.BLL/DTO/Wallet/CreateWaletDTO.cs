using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class CreateWaletDTO
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public CreateWaletDTO(bool isSuccess, string message)
        {
            this.isSuccess = isSuccess;
            this.Message = message;
        }


    }
}
