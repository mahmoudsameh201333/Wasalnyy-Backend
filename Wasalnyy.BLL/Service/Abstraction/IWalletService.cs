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
        Task<Wallet?> GetWalletByUserIdAsync(string userId);

        Task<IncreaseWalletBalanceResponse> IncreaseWalletAsync(string userId, decimal amount);
        Task<bool> WithdrawFromWalletAsync(string userId, decimal amount, string? reference = null);
    

        Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string? tripId = null);
        Task<bool> CheckUserBalanceAsync(string userId, decimal amount);
    }
}
