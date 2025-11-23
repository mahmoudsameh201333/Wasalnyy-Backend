using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IWalletRepo
    {





        Task<Wallet?> GetWalletOfUserIdAsync(string userId);

        Task UpdateWalletAsync(Wallet wallet);
        public Task UpdateWalletWithoutSaving(Wallet wallet);



        //4of dol ya mahmoud sameh  ana mo4 3ayzhom
        Task<Wallet?> GetByIdAsync(Guid walletId);
        Task CreateAsync(Wallet wallet);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
