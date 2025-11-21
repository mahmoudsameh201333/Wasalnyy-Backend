using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class WasalnyyHubService : IWasalnyyHubService
    {
        private readonly IWasalnyyHubConnectionRepo _repo;
        private readonly UserManager<User> _userManager;

        public WasalnyyHubService(IWasalnyyHubConnectionRepo repo, UserManager<User> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }
        public async Task CreateConnectionAsync(WasalnyyHubConnection connection)
        {
            await _repo.CreateAsync(connection);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAllConnectionsAsync()
        {
            await _repo.DeleteAllConnectionsAsync();
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteConnectionAsync(string signalRConnectionId)
        {
            await _repo.DeleteAsync(signalRConnectionId);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetAllUserConnectionsAsync(string userId)
        {
            return await _repo.GetAllUserConnectionsAsync(userId);
        }

        public async Task<string?> GetUserIdAsync(string signalRConnectionId)
        {
            return await _repo.GetUserIdAsync(signalRConnectionId);
        }

        public async Task<bool> IsOnlineAsync(string userId)
        {
            return await _repo.IsOnlineAsync(userId);
        }
    }
}
