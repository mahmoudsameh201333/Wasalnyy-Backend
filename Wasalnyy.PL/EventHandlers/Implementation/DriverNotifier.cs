namespace Wasalnyy.PL.EventHandlers.Implementation
{
    public class DriverNotifier : IDriverNotifier
    {
        private readonly IHubContext<WasalnyyHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DriverNotifier(IHubContext<WasalnyyHub> hubContext, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task OnDriverLocationUpdated(string driverId, Coordinates coordinates)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();

            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if(conId != null)
            {
                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();
                var trip = await _tripService.GetDriverActiveTripAsync(driverId);

                if(trip != null)
                {
                    if(trip.TripStatus.ToString() == TripStatus.Accepted.ToString())
                    {
                        await _hubContext.Clients.GroupExcept($"trip_{trip.Id}", conId).SendAsync("yourDriverLocationUpdated", coordinates);
                    }
                    else
                    {
                        await _tripService.UpdateTripLocationAsync(trip.Id, coordinates);
                        await _hubContext.Clients.Groups($"trip_{trip.Id}").SendAsync("tripLocationUpdated", coordinates);
                    }
                }
            }

        }

        public async Task OnDriverStatusChangedToAvailable(string driverId, Guid zoneId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();

            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if (conId != null)
            {
                await _hubContext.Groups.AddToGroupAsync(conId, $"driversAvailableInZone_{zoneId}");
                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();

                var tripsInZone = await _tripService.GetByRequestedTripsByZoneAsync(zoneId);
                foreach (var trip in tripsInZone)
                {
                    await _hubContext.Clients.Client(conId).SendAsync("availableTripsInZone", trip);
                }
            }
        }

        public async Task OnDriverStatusChangedToUnAvailable(string driverId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if (conId != null) 
            {
                var _driverService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDriverService>();

                var driver = await _driverService.GetByIdAsync(driverId);
                if (driver != null && driver.ZoneId != null)
                    await _hubContext.Groups.RemoveFromGroupAsync(conId, $"driversAvailableInZone_{driver.ZoneId}");
                await _connectionService.DeleteConnectionAsync(conId);
            }

        }

        public async Task OnDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();


            if (conId != null)
            {
                if (oldZoneId != null)
                    await _hubContext.Groups.RemoveFromGroupAsync(conId, $"driversAvailableInZone_{oldZoneId}");

                await _hubContext.Groups.AddToGroupAsync(conId, $"driversAvailableInZone_{newZoneId}");

                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();

                var tripsInZone = await _tripService.GetByRequestedTripsByZoneAsync(newZoneId);
                foreach (var trip in tripsInZone)
                {
                    await _hubContext.Clients.Client(conId).SendAsync("availableTripsInZone", trip);
                }
            }

        }
        public async Task OnDriverOutOfZone(string driverId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if(conId != null)
            {
                await _hubContext.Clients.Client(conId).SendAsync("outOfZone", "You are out of zone");
            }
        }
    }
}
