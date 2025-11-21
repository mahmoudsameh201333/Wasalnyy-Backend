using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class WasalnyyHubConnectionRepo : IWasalnyyHubConnectionRepo
    {
        private readonly WasalnyyDbContext _context;

        public WasalnyyHubConnectionRepo(WasalnyyDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(WasalnyyHubConnection connection)
        {
            await _context.WasalnyyHubConnections.AddAsync(connection);
        }

        public async Task DeleteAllConnectionsAsync()
        {
            _context.WasalnyyHubConnections.RemoveRange(_context.WasalnyyHubConnections);
        }

        public async Task DeleteAsync(string signalRConnectionId)
        {
            var connection = await _context.WasalnyyHubConnections.SingleOrDefaultAsync(x => x.SignalRConnectionId == signalRConnectionId);

            if (connection != null)
                _context.WasalnyyHubConnections.Remove(connection);
        }

        public async Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId)
        {
            return await _context.WasalnyyHubConnections.Where(x=> x.UserId == userId).Select(x=> x.SignalRConnectionId).ToListAsync();
        }

        public async Task<string?> GetUserIdAsync(string signalRConnectionId)
        {
            return (await _context.WasalnyyHubConnections.SingleOrDefaultAsync(x => x.SignalRConnectionId == signalRConnectionId))?.UserId;
        }

        public async Task<bool> IsOnlineAsync(string userId)
        {
            return (await _context.WasalnyyHubConnections.AnyAsync(x=> x.UserId == userId));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
