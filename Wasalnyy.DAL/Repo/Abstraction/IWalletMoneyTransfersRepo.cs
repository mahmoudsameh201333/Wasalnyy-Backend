using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IWalletMoneyTransfersRepo
    {
        public Task AddAsync(WalletMoneyTransfer transfer);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
