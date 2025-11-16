using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class WasalnyyHubConnectionService : IWasalnyyHubConnectionService
    {
        private readonly IWasalnyyHubConnectionRepo _repo;

        public WasalnyyHubConnectionService(IWasalnyyHubConnectionRepo repo)
        {
            _repo = repo;
        }
        public async Task CreateAsync(WasalnyyHubConnection connection)
        {
            await _repo.CreateAsync(connection);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAllConnectionsAsync()
        {
            await _repo.DeleteAllConnectionsAsync();
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string signalRConnectionId)
        {
            await _repo.DeleteAsync(signalRConnectionId);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId)
        {
            return await _repo.GetAllUserConnectionsAsync(userId);
        }

        public async Task<bool> IsOnlineAsync(string userId)
        {
            return await _repo.IsOnlineAsync(userId);
        }

    }
}
