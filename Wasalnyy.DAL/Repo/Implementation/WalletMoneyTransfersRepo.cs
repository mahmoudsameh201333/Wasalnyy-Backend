using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public  class WalletMoneyTransfersRepo: IWalletMoneyTransfersRepo
    {
        private readonly DbContext _context;

        public WalletMoneyTransfersRepo(WasalnyyDbContext context )
        {
            _context = context;
        }

        public async Task AddAsync(WalletMoneyTransfer transfer)
        {
            transfer.Id = Guid.NewGuid();
            await _context.Set<WalletMoneyTransfer>().AddAsync(transfer);
        }

       
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
