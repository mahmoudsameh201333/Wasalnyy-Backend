using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class ChatHubRepo: IChatHubRepo
    {
        private readonly WasalnyyDbContext _context;

        public ChatHubRepo(WasalnyyDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(ChatHubConnection connection)
        {
           await _context.ChatHubConnections.AddAsync(connection);
        }

        public async Task DeleteAllConnectionsAsync()
        {
            _context.ChatHubConnections.RemoveRange(_context.ChatHubConnections);
        }

        public async Task DeleteAsync(string signalRConnectionId)
        {
            var connection = await _context.ChatHubConnections.SingleOrDefaultAsync(x => x.SignalRConnectionId == signalRConnectionId);

            if (connection != null)
                _context.ChatHubConnections.Remove(connection);
        }

        public async Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId)
        {
            return await _context.ChatHubConnections.Where(x => x.UserId == userId).Select(x => x.SignalRConnectionId).ToListAsync();
        }

        public async Task<string?> GetUserIdAsync(string signalRConnectionId)
        {
            return (await _context.ChatHubConnections.SingleOrDefaultAsync(x => x.SignalRConnectionId == signalRConnectionId))?.UserId;
        }

        public async Task<bool> IsOnlineAsync(string userId)
        {
            return (await _context.ChatHubConnections.AnyAsync(x => x.UserId == userId));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

