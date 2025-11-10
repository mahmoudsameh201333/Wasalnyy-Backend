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
        Task<TripDto> GetByIdAsync(Guid id);
        Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(string zone);
        Task<TripPaginationDto> GetAllAsync(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task<TripPaginationDto> GetAllRiderTripsAsync(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);

        Task<TripPaginationDto> GetAllDriverTripsAsync(string driverId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task RequestTripAsync(string userId, RequestTripDto dto);
        Task AcceptTripAsync(Guid driverId, Guid tripId);
        Task EndTripAsync(Guid tripId);
        Task CancelTripAsync(Guid tripId);
        Task<IEnumerable<TripDto>> GetRequestedTripsByZone(Guid zoneId);
    }
}
