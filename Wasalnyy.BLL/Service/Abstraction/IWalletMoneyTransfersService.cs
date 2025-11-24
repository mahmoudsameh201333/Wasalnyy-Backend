using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IWalletMoneyTransfersService
    {

        public Task <TransferWalletResponse> AddAsync(AddWalletTranferMoneyDTO addWalletTranferMoneyDTO);

    }
}
