using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ChatHubService: IChatHubService
    {
        private readonly IChatHubRepo _repo;
        private readonly UserManager<User> _userManager;

        public ChatHubService(IChatHubRepo repo, UserManager<User> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }
        public async Task CreateConnectionAsync(ChatHubConnection connection)
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
