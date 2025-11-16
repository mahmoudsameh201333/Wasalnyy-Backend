using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IWasalnyyHubConnectionService
    {
        Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId);
        Task CreateAsync(WasalnyyHubConnection connection);
        Task DeleteAsync(string signalRConnectionId);
        Task DeleteAllConnectionsAsync();
        Task<bool> IsOnlineAsync(string userId);
    }
}
