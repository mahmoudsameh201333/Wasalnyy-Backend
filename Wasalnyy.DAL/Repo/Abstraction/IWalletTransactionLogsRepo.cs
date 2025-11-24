using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IWalletTransactionLogsRepo
    {
        Task CreateAsync(WalletTransactionLogs transaction);
        Task<IEnumerable<WalletTransactionLogs>> GetByWalletIdAsync(Guid walletId);
        Task<WalletTransactionLogs?> GetByIdAsync(Guid transactionId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
