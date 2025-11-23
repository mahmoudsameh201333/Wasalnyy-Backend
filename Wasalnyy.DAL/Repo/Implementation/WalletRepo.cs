using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class WalletRepo : IWalletRepo
    {
        private readonly WasalnyyDbContext _context;

        public WalletRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(string userId)
        {
            return await _context.Wallets
                .AsNoTracking()
                .Include(w => w.Transactions)
                .SingleOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet?> GetByIdAsync(Guid walletId)
        {
            return await _context.Wallets
                .AsNoTracking()
                .Include(w => w.Transactions)
                .SingleOrDefaultAsync(w => w.Id == walletId);
        }

        public async Task CreateAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Entry(wallet).State = EntityState.Modified;
            await _context.SaveChangesAsync();  // <-- this actually updates the table
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
