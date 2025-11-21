using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.PL.Hubs;

namespace Wasalnyy.PL.EventHandlers.Implementation
{
    public class WasalnyyHubNotifier : IWasalnyyHubNotifier
    {
        private readonly IHubContext<WasalnyyHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WasalnyyHubNotifier(IHubContext<WasalnyyHub> hubContext, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task OnUserConnected(string userId, string connectionId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var _userManager = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Driver"))
            {
                if (await _connectionService.IsOnlineAsync(userId))
                    throw new AlreadyLoggedInWithAnotherDeviceException("You already logged in with another device.");
                await _connectionService.CreateConnectionAsync(new WasalnyyHubConnection { SignalRConnectionId = connectionId, UserId = userId });

                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();

                var activeTrip = await _tripService.GetDriverActiveTripAsync(userId);

                if (activeTrip != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, $"trip_{activeTrip.Id}");
                    await _hubContext.Clients.Client(connectionId).SendAsync("pendingTrip", activeTrip);
                }
                else
                {
                    var _driverService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDriverService>();
                    var driver = await _driverService.GetByIdAsync(userId);

                    if (driver != null && driver.DriverStatus == DriverStatus.Available)
                        await _driverService.SetDriverUnAvailableAsync(userId);
                }
                    return;
            }
            else if (roles.Contains("Rider"))
            {
                await _connectionService.CreateConnectionAsync(new WasalnyyHubConnection { SignalRConnectionId = connectionId, UserId = userId });

                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();

                var activeTrip =  await _tripService.GetRiderActiveTripAsync(userId);

                if (activeTrip != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, $"trip_{activeTrip.Id}");
                    await _hubContext.Clients.Client(connectionId).SendAsync("pendingTrip", activeTrip);
                }
                return;
            }

            throw new UnauthorizedAccessException();            
        }

        public async Task OnUserDisconnected(string connectionId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var userId = await _connectionService.GetUserIdAsync(connectionId);


            var _userManager = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            var roles = await _userManager.GetRolesAsync(user);


            if (roles.Contains("Driver"))
            {
                var _driverService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDriverService>();
                var driver = await _driverService.GetByIdAsync(userId);

                if(driver != null && driver.DriverStatus == DriverStatus.Available)
                    await _driverService.SetDriverUnAvailableAsync(userId);
            }

            await _connectionService.DeleteConnectionAsync(connectionId);

        }
    }
}
