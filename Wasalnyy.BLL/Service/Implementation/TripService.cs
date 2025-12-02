namespace Wasalnyy.BLL.Service.Implementation
{
    public class TripService : ITripService
    {
        private readonly TripEvents _tripEvents;
        private readonly ITripRepo _tripRepo;
        private readonly IDriverService _driverService;
        private readonly IRiderService _riderService;
        private readonly IRouteService _routeService;
        private readonly IPricingService _pricingService;
        private readonly IZoneService _zoneService;
        private readonly IWalletService _walletService;
        private readonly TripServiceValidator _validator;
        private readonly IMapper _mapper;

        public TripService(ITripRepo tripRepo, IDriverService driverService, TripEvents tripEvents,
            IRiderService riderService, IRouteService routeService, IPricingService pricingService,
            IZoneService zoneService, TripServiceValidator tripServiceValidator, IWalletService walletService, IMapper mapper)
        {
            _tripRepo = tripRepo;
            _driverService = driverService;
            _tripEvents = tripEvents;
            _riderService = riderService;
            _routeService = routeService;
            _pricingService = pricingService;
            _zoneService = zoneService;
            _validator = tripServiceValidator;
            _walletService = walletService;
            _mapper = mapper;
        }

        public async Task RequestTripAsync(string riderId, RequestTripDto dto)
        {
            await _validator.ValidateRequestTrip(riderId, dto);

            var rider = await _riderService.GetByIdAsync(riderId);
            if(rider == null)
                throw new NotFoundException($"Rider with ID '{riderId}' was not found.");

            var riderActiveTripe = await GetRiderActiveTripAsync(riderId);

            if(riderActiveTripe != null)
                throw new AlreadyInTripException($"Rider with id {riderId} already in trip.");


            var trip = _mapper.Map<RequestTripDto, Trip>(dto);

            var pickupZone = await _zoneService.GetZoneAsync(trip.PickupCoordinates);

            if (pickupZone == null)
                throw new OutOfZoneException("Trip is out of zone.");

            var distinationZone = await _zoneService.GetZoneAsync(trip.DestinationCoordinates);

            if (distinationZone == null)
                throw new OutOfZoneException("Trip is out of zone.");

            trip.ZoneId = pickupZone.Id;
            trip.RiderId = riderId;

            (trip.DistanceKm, trip.DurationMinutes) = await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.DestinationCoordinates);
            trip.Price = await _pricingService.CalculatePriceAsync(_mapper.Map<Trip, CalculatePriceDto>(trip));

            if (dto.PaymentMethod == PaymentMethod.Wallet && !await _walletService.CheckUserBalanceAsync(riderId, (decimal)trip.Price))
                throw new WalletBalanceNotSufficientException($"Your wallet balance not sufficient");

            await _tripRepo.CreateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _tripEvents.FireTripRequested(_mapper.Map<Trip, TripDto>(trip));
        }
        public async Task ConfirmTripAsync(string riderId, Guid tripId)
        {
            await _validator.ValidateConfirmTrip(riderId, tripId);

            var rider = await _riderService.GetByIdAsync(riderId);
            if (rider == null)
                throw new NotFoundException($"Rider with ID '{riderId}' was not found.");

            var trip = await _tripRepo.GetByIdAsync(tripId);

            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Requested)
                throw new InvalidOperationException($"Trip {tripId} is not in a requestable state. Current status: {trip.TripStatus}");


            if (rider.RiderId != trip.RiderId)
                throw new InvalidOperationException($"rider with id {rider.RiderId} can not confirm another rider trip");

            trip.TripStatus = TripStatus.Confirmed;
            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _tripEvents.FireTripConfirmed(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task AcceptTripAsync(string driverId, Guid tripId)
        {
            await _validator.ValidateAcceptTrip(driverId, tripId);

            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Confirmed)
                throw new InvalidOperationException($"Trip {tripId} is not in a requestable state. Current status: {trip.TripStatus}");


            if (!string.IsNullOrEmpty(trip.DriverId))
                throw new TripAlreadyAssignedToDriver($"Trip with ID '{tripId}' already asigned to driver."); 

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            var driverActiveTrip = await GetDriverActiveTripAsync(driverId);

            if (driverActiveTrip != null)
                throw new AlreadyInTripException($"Driver with id {driverId} already in trip.");

            trip.DriverId = driverId;
            trip.TripStatus = TripStatus.Accepted;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _driverService.SetDriverInTripAsync(driverId);

            await _tripEvents.FireTripAccepted(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task StartTripAsync(string driverId, Guid tripId)
        {
            await _validator.ValidateStartTrip(driverId, tripId);

            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Accepted)
                throw new InvalidOperationException($"Trip {tripId} cannot be started from status {trip.TripStatus}");

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if(trip.DriverId !=  driverId)
                throw new DriverMismatchException($"Driver '{driverId}' is not assigned to trip '{tripId}'.");

            trip.TripStatus = TripStatus.Started;
            trip.StartDate = DateTime.UtcNow;

            trip.CurrentCoordinates = driver.Coordinates;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _tripEvents.FireTripStarted(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task EndTripAsync(string driverId, Guid tripId)
        {
            await _validator.ValidateEndTrip(driverId, tripId);

            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Started)
                throw new InvalidOperationException($"Trip {tripId} cannot be Ended from status {trip.TripStatus}");

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if (trip.DriverId != driverId)
                throw new DriverMismatchException($"Driver '{driverId}' is not assigned to trip '{tripId}'.");

            if (trip.PaymentMethod == PaymentMethod.Wallet)
            {
                var transferTransaction = await _walletService.HandleTransferWalletMoneyFromRiderToDriver(_mapper.Map<Trip, TransferMoneyBetweenUsersDTO>(trip));
                if (!transferTransaction.IsSuccess)
                    throw new WalletTransferException(transferTransaction.Message);
            }
                

            trip.TripStatus = TripStatus.Ended;
            trip.ArrivalDate = DateTime.UtcNow;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _driverService.SetDriverAvailableAsync(driverId, driver.Coordinates);

            await _tripEvents.FireTripEnded(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task CancelTripAsync(string userId, Guid tripId)
        {
            await _validator.ValidateCancelTripAsync(userId, tripId);
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus == TripStatus.Ended || trip.TripStatus == TripStatus.Cancelled)
            {
                throw new InvalidOperationException("Trip is already Ended or Cancelled");
            }


            if (trip.DriverId != userId && trip.RiderId != userId)
                throw new InvalidOperationException($"UserId did not match wiht any rider or driver");

            var oldStatus = trip.TripStatus;
            CashCancelationFees? cashCancelationFees = null;

            if (trip.TripStatus == TripStatus.Started)
            {
                (double distanceKm, double durationMinutes) = await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.CurrentCoordinates);
                double distanceTraveledCost = await _pricingService.CalculatePriceAsync(new CalculatePriceDto { DistanceKm = distanceKm, DurationMinutes = durationMinutes });

                if (trip.PaymentMethod == PaymentMethod.Wallet)
                {
                    var transferDto = _mapper.Map<Trip, TransferMoneyBetweenUsersDTO>(trip);
                    transferDto.Amount = (decimal)distanceTraveledCost;

                    var transferTransaction = await _walletService.HandleTransferWalletMoneyFromRiderToDriver(transferDto);

                    if (!transferTransaction.IsSuccess)
                        throw new WalletTransferException(transferTransaction.Message);

                    var withdrawRes = await _walletService.WithdrawFromWalletAsync(new WithdrawFromWalletDto { UserId = userId, Amount = (decimal)(trip.Price * 0.01), Description = $"Cancellation fee", CreatedAt = DateTime.UtcNow });

                    if (!withdrawRes.IsSuccess)
                        throw new WithdrawException(withdrawRes.Message);

                }
                else if (trip.PaymentMethod == PaymentMethod.Cash)
                {
                    cashCancelationFees = new CashCancelationFees { DistanceTraveledCost = distanceTraveledCost, Fees = 0 };

                    if (trip.RiderId == userId)
                    {
                        cashCancelationFees.Fees = trip.Price * 0.01;
                    }
                    else
                    {
                        var withdrawRes = await _walletService.WithdrawFromWalletAsync(new WithdrawFromWalletDto { UserId = userId, Amount = (decimal)(trip.Price * 0.01), Description = $"Cancellation fee", CreatedAt = DateTime.UtcNow });

                        if (!withdrawRes.IsSuccess)
                            throw new WithdrawException(withdrawRes.Message);
                    }
                }

            }

            trip.TripStatus = TripStatus.Cancelled;
            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _tripEvents.FireTripCanceled(_mapper.Map<Trip, TripDto>(trip), oldStatus, cashCancelationFees);

            if (!string.IsNullOrWhiteSpace(trip.DriverId))
            {
                var driver = await _driverService.GetByIdAsync(trip.DriverId);
                await _driverService.SetDriverAvailableAsync(trip.DriverId, driver.Coordinates);
            }
        }

        public async Task UpdateTripLocationAsync(Guid tripId, Coordinates coordinates)
        {
            await _validator.ValidateUpdateTripLocation(tripId, coordinates);

            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Started)
                throw new InvalidOperationException($"Trip location cannot be updated.");

            trip.CurrentCoordinates = coordinates;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();
        }

        public async Task<TripDto?> GetDriverActiveTripAsync(string driverId)
        {
            await _validator.ValidateGetDriverActiveTrip(driverId);

            return _mapper.Map<Trip?, TripDto?>(await _tripRepo.GetDriverActiveTripAsync(driverId));
        }

        public async Task<TripDto?> GetRiderActiveTripAsync(string riderId)
        {
            await _validator.ValidateGetRiderActiveTrip(riderId);

            return _mapper.Map<Trip?, TripDto?>(await _tripRepo.GetRiderActiveTripAsync(riderId));
        }

        public async Task<TripPaginationDto> GetAllPaginatedAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            await _validator.ValidateGetAllPaginated(orderBy, descending, pageNumber,pageSize);

            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize > 100)
                pageSize = 100;

            if (pageSize <= 0)
                pageSize = 10;

            var totalPageCount = await GetPagesCountAsync(pageSize);

            if (totalPageCount == 0 || pageNumber > totalPageCount)
                return new TripPaginationDto
                {
                    CurrentPage = 0,
                    PageSize = 0,
                    TotalPages = 0,
                    Trips = new List<TripDto>()
                };

            var trips = (await _tripRepo.GetTripsPaginatedAsync(orderBy, descending, pageNumber, pageSize)).ToList();

            return new TripPaginationDto
            {
                Trips = _mapper.Map<List<Trip>, List<TripDto>>(trips),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPageCount
            };
        }

        public async Task<TripPaginationDto> GetAllDriverTripsPaginatedAsync(string driverId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            await _validator.ValidateGetAllDriverTripsPaginated(driverId, orderBy, descending, pageNumber, pageSize);

            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize > 100)
                pageSize = 100;

            if (pageSize <= 0)
                pageSize = 10;

            var totalPageCount = await GetDriverTripsPagesCountAsync(driverId, pageSize);

            if (totalPageCount == 0 || pageNumber > totalPageCount)
                return new TripPaginationDto
                {
                    CurrentPage = 0,
                    PageSize = 0,
                    TotalPages = 0,
                    Trips = new List<TripDto>()
                };

            var trips = (await _tripRepo.GetAllDriverTripsPaginatedAsync(driverId ,orderBy, descending, pageNumber, pageSize)).ToList();

            return new TripPaginationDto
            {
                Trips = _mapper.Map<List<Trip>, List<TripDto>>(trips),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPageCount
            };
        }

        public async Task<TripPaginationDto> GetAllRiderTripsPaginatedAsync(string riderId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            await _validator.ValidateGetAllRiderTripsPaginated(riderId, orderBy, descending, pageNumber, pageSize);

            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize > 100)
                pageSize = 100;

            if (pageSize <= 0)
                pageSize = 10;

            var totalPageCount = await GetRiderTripsPagesCountAsync(riderId, pageSize);

            if (totalPageCount == 0 || pageNumber > totalPageCount)
                return new TripPaginationDto
                {
                    CurrentPage = 0,
                    PageSize = 0,
                    TotalPages = 0,
                    Trips = new List<TripDto>()
                };

            var trips = (await _tripRepo.GetAllRiderTripsPaginatedAsync(riderId, orderBy, descending, pageNumber, pageSize)).ToList();

            return new TripPaginationDto
            {
                Trips = _mapper.Map<List<Trip>, List<TripDto>>(trips),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPageCount
            };
        }

        public async Task<TripDto?> GetByIdAsync(Guid id)
        {
            await _validator.ValidateGetById(id);

            var trip = await _tripRepo.GetByIdAsync(id);
            if(trip == null)
                throw new NotFoundException($"Trip with ID '{id}' was not found.");

            return _mapper.Map<Trip, TripDto>(trip);
        }

        public async Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(Guid zoneId)
        {
            await _validator.ValidateGetByRequestedTripsByZone(zoneId);
            return _mapper.Map<IEnumerable<Trip>, IEnumerable<TripDto>>( await _tripRepo.GetAvailableTripsByZoneAsync(zoneId));
        }

        public async Task<int> GetPagesCountAsync(int pageSize = 10)
        {
            await _validator.ValidateGetPagesCount(pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetCountAsync()) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetRiderTripsPagesCountAsync(string riderId, int pageSize = 10)
        {
            await _validator.ValidateGetRiderTripsPagesCount(riderId, pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetRiderTripsCountAsync(riderId)) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetDriverTripsPagesCountAsync(string driverId, int pageSize = 10)
        {
            await _validator.ValidateGetDriverTripsPagesCount(driverId, pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetDriverTripsCountAsync(driverId)) / (double)pageSize);

            return (int)noPages;
        }
        
    }
}
