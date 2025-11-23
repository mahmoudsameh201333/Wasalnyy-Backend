using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IWalletService
    {
        //4of deh ya mahmoud sameh mo4 3ayzha 
        Task<Wallet?> GetWalletByUserIdAsync(string userId);

        Task<IncreaseWalletBalanceResponse> IncreaseWalletAsync(IncreaseWalletDTO increaseWalletDTO);

        //Task<bool> WithdrawFromWalletAsync(string userId, decimal amount, string? reference = null);
    
        Task<CreateWalletResponse>CreateWallet(CreateWalletDTO createWalletDTO);
        Task<TransferWalletResponse> HandleTransferWalletMoneyFromRiderToDriver(TransferMoneyBetweenUsersDTO transferDto);

        //w deh mo4 3ayzha
        // Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string? tripId = null);
    }
}
