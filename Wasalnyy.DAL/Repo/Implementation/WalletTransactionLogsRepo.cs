using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class WalletTransactionLogsRepo : IWalletTransactionLogsRepo
    {
        private readonly WasalnyyDbContext _context;

        public WalletTransactionLogsRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(WalletTransactionLogs transaction)
        {
            transaction.Id = Guid.NewGuid();
            await _context.WalletTransactions.AddAsync(transaction);
        }

        public async Task<WalletTransactionLogs?> GetByIdAsync(Guid transactionId)
        {
            return await _context.WalletTransactions
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<IEnumerable<WalletTransactionLogs>> GetByWalletIdAsync(Guid walletId)
        {
            return await _context.WalletTransactions
                .AsNoTracking()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
