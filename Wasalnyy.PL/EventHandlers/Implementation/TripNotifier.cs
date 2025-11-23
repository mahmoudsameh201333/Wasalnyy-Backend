using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.PL.Hubs;

namespace Wasalnyy.PL.EventHandlers.Implementation
{
    public class TripNotifier : ITripNotifier
    {
        private readonly IHubContext<WasalnyyHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TripNotifier(IHubContext<WasalnyyHub> hubContext, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task OnTripRequested(TripDto dto)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var riderConIds = await _connectionService.GetAllUserConnectionsAsync(dto.RiderId);

            if (riderConIds.Count() > 0)
            {
                foreach (var conId in riderConIds)
                {
                    await _hubContext.Clients.Client(conId).SendAsync("tripRequested", dto);
                }
            }
        }

        public async Task OnTripAccepted(TripDto dto)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            var _driverService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDriverService>();
            var riderConIds = await _connectionService.GetAllUserConnectionsAsync(dto.RiderId);
            var driverConId = (await _connectionService.GetAllUserConnectionsAsync(dto.DriverId)).FirstOrDefault();

            if (driverConId != null && riderConIds.Count() > 0)
            {
                await _hubContext.Groups.RemoveFromGroupAsync(driverConId, $"driversAvailableInZone_{dto.ZoneId}");
                await _hubContext.Clients.Group($"driversAvailableInZone_{dto.ZoneId}").SendAsync("tripAcceptedFromAnotherDriver", dto.Id);

                await _hubContext.Groups.AddToGroupAsync(driverConId, $"trip_{dto.Id}");

                var driver = await _driverService.GetByIdAsync(dto.DriverId);
                foreach (var conId in riderConIds)
                {
                    await _hubContext.Groups.AddToGroupAsync(conId, $"trip_{dto.Id}");

                    await _hubContext.Clients.Client(conId).SendAsync("tripAccepeted", driver);
                }
            }
        }

        public async Task OnTripCanceled(TripDto dto)
        {
			var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();

			await _hubContext.Clients.Group($"trip_{dto.Id}").SendAsync("tripCanceled", dto);

			var riderConIds = await _connectionService.GetAllUserConnectionsAsync(dto.RiderId);
			var driverConId = (await _connectionService.GetAllUserConnectionsAsync(dto.DriverId)).FirstOrDefault();

			if (driverConId != null && riderConIds.Count() > 0)
			{
				await _hubContext.Groups.RemoveFromGroupAsync(driverConId, $"trip_{dto.Id}");

				foreach (var conId in riderConIds)
				{
					await _hubContext.Groups.RemoveFromGroupAsync(conId, $"trip_{dto.Id}");

				}
			}
		}

        public async Task OnTripEnded(TripDto dto)
        {
            var _connectionService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IWasalnyyHubService>();
            
            await _hubContext.Clients.Group($"trip_{dto.Id}").SendAsync("tripEnded", dto);

            var riderConIds = await _connectionService.GetAllUserConnectionsAsync(dto.RiderId);
            var driverConId = (await _connectionService.GetAllUserConnectionsAsync(dto.DriverId)).FirstOrDefault();

            if (driverConId != null && riderConIds.Count() > 0)
            {
                await _hubContext.Groups.RemoveFromGroupAsync(driverConId, $"trip_{dto.Id}");

                foreach (var conId in riderConIds)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(conId, $"trip_{dto.Id}");

                }
            }
        }

        

        public async Task OnTripStarted(TripDto dto)
        {
            await _hubContext.Clients.Group($"trip_{dto.Id}").SendAsync("tripStarted", dto);
        }

        public async Task OnTripConfirmed(TripDto dto)
        {
            await _hubContext.Clients.Group($"driversAvailableInZone_{dto.ZoneId}").SendAsync("availableTripsInZone", dto);
        }
    }
}
