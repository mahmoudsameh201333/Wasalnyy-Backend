namespace Wasalnyy.BLL.Validators
{
    public class TripServiceValidator
    {
        public async Task ValidateGetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"id '{id}' is empty");
        }
        public async Task ValidateCancelTripAsync(string UserId, Guid tripId)
		{
			if (string.IsNullOrWhiteSpace(UserId))
				throw new ArgumentException(UserId);

			if (tripId == Guid.Empty)
				throw new ArgumentException($"tripId '{tripId}' is empty");
		}

        public async Task ValidateConfirmTrip(string riderId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public async Task ValidateGetByRequestedTripsByZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }
        public async Task ValidateGetDriverActiveTrip(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public async Task ValidateGetRiderActiveTrip(string riderId)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);
        }
        public async Task ValidateGetAllPaginated(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            ArgumentNullException.ThrowIfNull(orderBy);
        }
        public async Task ValidateGetAllRiderTripsPaginated(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            ArgumentNullException.ThrowIfNull(orderBy);

        }

        public async Task ValidateGetAllDriverTripsPaginated(string driverId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            ArgumentNullException.ThrowIfNull(orderBy);
        }
        public async Task ValidateRequestTrip(string riderId, RequestTripDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

        }
        public async Task ValidateAcceptTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public async Task ValidateStartTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
        public async Task ValidateEndTrip(string driverId, Guid tripId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }

        public async Task ValidateGetPagesCount(int pageSize)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public async Task ValidateGetRiderTripsPagesCount(string riderId, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(riderId))
                throw new ArgumentException(riderId);

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public async Task ValidateGetDriverTripsPagesCount(string driverId, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
        }

        public async Task ValidateUpdateTripLocation(Guid tripId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (tripId == Guid.Empty)
                throw new ArgumentException($"tripId '{tripId}' is empty");
        }
    }
}
