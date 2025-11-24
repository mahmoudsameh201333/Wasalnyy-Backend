using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface ITripService
    {
        Task<TripDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(Guid zoneId);
        Task<TripDto?> GetDriverActiveTripAsync(string driverId);
        Task<TripDto?> GetRiderActiveTripAsync(string riderId);
        Task<TripPaginationDto> GetAllPaginatedAsync(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task<TripPaginationDto> GetAllRiderTripsPaginatedAsync(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);

        Task<TripPaginationDto> GetAllDriverTripsPaginatedAsync(string driverId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task RequestTripAsync(string riderId, RequestTripDto dto);
        Task ConfirmTripAsync(string riderId, Guid tripId);
        Task AcceptTripAsync(string driverId, Guid tripId);
        Task StartTripAsync(string driverId, Guid tripId);
        Task EndTripAsync(string driverId, Guid tripId);
        Task CancelTripAsync(string UserId, Guid tripId);
        Task UpdateTripLocationAsync(Guid tripId, Coordinates coordinates);
    }
}
