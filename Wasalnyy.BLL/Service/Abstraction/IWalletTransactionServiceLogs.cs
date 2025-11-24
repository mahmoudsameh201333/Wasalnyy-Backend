using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IWalletTransactionService
    {
        Task <CreateWalletTransactionLogResponse> CreateAsync(CreateWalletTransactionDTO createWalletTransactionDTO);

    }
}
