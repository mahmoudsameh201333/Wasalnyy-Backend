using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Validators
{
    public class TripServiceValidator
    {
        public void ValidateGetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"id '{id}' is empty");
        }
        public void ValidateCancelTripAsync(string UserId, Guid tripId)
		{
			if (string.IsNullOrWhiteSpace(UserId))
				throw new ArgumentException(UserId);

			if (tripId == Guid.Empty)
				throw new ArgumentException($"tripId '{tripId}' is empty");
		}

        public void ValidateConfirmTrip(string riderId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public void ValidateGetByRequestedTripsByZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }
        public void ValidateGetDriverActiveTrip(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public void ValidateGetRiderActiveTrip(string riderId)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);
        }
        public void ValidateGetAllPaginated(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            ArgumentNullException.ThrowIfNull(orderBy);
        }
        public void ValidateGetAllRiderTripsPaginated(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            ArgumentNullException.ThrowIfNull(orderBy);

        }

        public void ValidateGetAllDriverTripsPaginated(string driverId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            ArgumentNullException.ThrowIfNull(orderBy);
        }
        public void ValidateRequestTrip(string riderId, RequestTripDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

        }
        public void ValidateAcceptTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public void ValidateStartTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public void ValidateEndTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }

        public void ValidateGetPagesCount(int pageSize)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public void ValidateGetRiderTripsPagesCount(string riderId, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public void ValidateGetDriverTripsPagesCount(string driverId, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public void ValidateUpdateTripLocation(Guid tripId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
    }
}
