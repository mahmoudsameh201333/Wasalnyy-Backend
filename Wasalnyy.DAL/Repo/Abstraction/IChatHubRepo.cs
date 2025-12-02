using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IChatHubRepo
    {
        Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId);
        Task CreateAsync(ChatHubConnection connection);
        Task DeleteAsync(string signalRConnectionId);
        Task DeleteAllConnectionsAsync();
        Task<bool> IsOnlineAsync(string userId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<string?> GetUserIdAsync(string signalRConnectionId);
    }
}
