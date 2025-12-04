using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.PL.Hubs;

namespace Wasalnyy.PL.EventHandlers.Implementation
{
    public class ChatHubEventHandler : IChatHubEventHandler
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
       
        public ChatHubEventHandler(IHubContext<ChatHub> hubContext, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }
        

        public async Task OnUserConnected(string userId, string connectionId)
        {


            using var scope = _serviceScopeFactory.CreateScope();
            var _connectionService = scope.ServiceProvider.GetRequiredService<IChatHubService>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();


            var roles = await _userManager.GetRolesAsync(user);


            if (!roles.Contains("Driver") && !roles.Contains("Rider"))
                throw new UnauthorizedAccessException("You are not allowed to access this hub.");

            if (roles.Contains("Driver"))
            {
                if (await _connectionService.IsOnlineAsync(userId))
                    throw new AlreadyLoggedInWithAnotherDeviceException("You already logged in with another device.");

            }

            await _connectionService.CreateConnectionAsync(new ChatHubConnection { SignalRConnectionId = connectionId, UserId = userId });


        
        } 

        public async Task OnUserDisconnected(string connectionId)
        {

            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IChatHubService>();
            //Delete connection 
            await _connectionService.DeleteConnectionAsync(connectionId);

        }


        public async Task OnSendMessage(string senderId, string receiverId, string messageContent, string senderConnectionId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var _messageService = scope.ServiceProvider.GetRequiredService<IChatService>();
            var _connectionService = scope.ServiceProvider.GetRequiredService<IChatHubService>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // 1. Validate both users exist
            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (sender == null)
                throw new NotFoundException($"Sender with ID {senderId} not found");
            if (receiver == null)
                throw new NotFoundException($"Receiver with ID {receiverId} not found");

            // 2. Validate both users are Driver or Rider
            var senderRoles = await _userManager.GetRolesAsync(sender);
            var receiverRoles = await _userManager.GetRolesAsync(receiver);

            if (!senderRoles.Contains("Driver") && !senderRoles.Contains("Rider"))
                throw new UnauthorizedAccessException("Sender must be a Driver or Rider");

            if (!receiverRoles.Contains("Driver") && !receiverRoles.Contains("Rider"))
                throw new UnauthorizedAccessException("Receiver must be a Driver or Rider");

            // 3. Save the message to database
            var messageDto = await _messageService.SendMessageAsync(senderId, receiverId, messageContent);

            // 4. Get all sender connection IDs
            var senderConnectionIds = await _connectionService.GetAllUserConnectionsAsync(senderId);

            // If sender is a Driver and has more than one connection, throw exception
            if (senderRoles.Contains("Driver") && senderConnectionIds.Count() > 1)
            {
                throw new AlreadyLoggedInWithAnotherDeviceException("Driver cannot be logged in on multiple devices");
            }

            // Send to sender's other devices (excluding the one that sent the message)
            var otherSenderConnections = senderConnectionIds.Where(id => id != senderConnectionId).ToList();
            if (otherSenderConnections.Any())
            {
                await _hubContext.Clients.Clients(otherSenderConnections)
                    .SendAsync("messagesent", messageDto);

            }

            // 5. Get all receiver connection IDs and send if online
            var receiverConnectionIds = await _connectionService.GetAllUserConnectionsAsync(receiverId);
            if (receiverConnectionIds.Any())
            {
                await _hubContext.Clients.Clients(receiverConnectionIds)
     .SendAsync("receivemessage", messageDto, receiverId);
            }
        }
    }
}
