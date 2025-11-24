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
        Task<Wallet?> GetWalletOfUserIdAsync(string userId);

        Task<IncreaseWalletBalanceResponse> IncreaseWalletAsync(IncreaseWalletDTO increaseWalletDTO);

        Task<WithDrawFromWalletResponse> WithdrawFromWalletAsync(WithdrawFromWalletDto withdrawFromWalletDto);
    
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletDTO createWalletDTO);
        Task<TransferWalletResponse> HandleTransferWalletMoneyFromRiderToDriver(TransferMoneyBetweenUsersDTO transferDto);

        Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string? tripId = null);
        Task<bool> CheckUserBalanceAsync(string userId, decimal amount);
    }
}
