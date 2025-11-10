using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class TripService : ITripService
    {
        public Task AcceptTripAsync(Guid driverId, Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task CancelTripAsync(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task EndTripAsync(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task<TripPaginationDto> GetAllAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TripPaginationDto> GetAllDriverTripsAsync(string driverId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TripPaginationDto> GetAllRiderTripsAsync(string riderId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TripDto> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(string zone)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TripDto>> GetRequestedTripsByZone(Guid zoneId)
        {
            throw new NotImplementedException();
        }

        public Task RequestTripAsync(string userId, RequestTripDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
