using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IWasalnyyHubService
    {
        Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId);
        Task<string?> GetUserIdAsync(string signalRConnectionId);
        Task CreateConnectionAsync(WasalnyyHubConnection connection);
        Task DeleteConnectionAsync(string signalRConnectionId);
        Task DeleteAllConnectionsAsync();
        Task<bool> IsOnlineAsync(string userId);
    }
}
