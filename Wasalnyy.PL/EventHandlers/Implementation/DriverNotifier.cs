using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.PL.Hubs;

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

            //_connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubConnectionService>();
            //_tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();
        }

        public async Task OnDriverLocationUpdated(string driverId, Coordinates coordinates)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubConnectionService>();

            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if(conId != null)
            {
                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();
                var trip = await _tripService.GetDriverActiveTripAsync(driverId);
                if(trip != null)
                {
                    await _hubContext.Clients.Groups($"trip_{trip.Id}").SendAsync("tripLocationUpdated", coordinates);
                }
            }

        }

        public async Task OnDriverStatusChangedToAvailable(string driverId, Guid zoneId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubConnectionService>();

            var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

            if (conId != null)
            {
                await _hubContext.Groups.AddToGroupAsync(conId, $"driversAvailableInZone_{zoneId}");
                var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();

                var tripsInZone = await _tripService.GetByRequestedTripsByZoneAsync(zoneId);
                foreach (var trip in tripsInZone)
                {
                    await _hubContext.Clients.Client(conId).SendAsync("availableTripsInZone", tripsInZone);
                }
            }
        }

        //public async Task OnDriverStatusChangedToInTrip(string driverId, Guid tripId)
        //{
        //    var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubConnectionService>();

        //    var conId = (await _connectionService.GetAllUserConnectionsAsync(driverId)).FirstOrDefault();

        //    if(conId != null) 
        //    {
        //        await _hubContext.Groups.AddToGroupAsync(conId, $"trip_{tripId}");

        //    }
        //}

        public async Task OnDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubConnectionService>();
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
    }
}
