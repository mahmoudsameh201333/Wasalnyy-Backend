using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Pricing;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.Enents;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Validators;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;

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
            _validator.ValidateRequestTrip(riderId, dto);

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

            var distinationZone = await _zoneService.GetZoneAsync(trip.DistinationCoordinates);

            if (distinationZone == null)
                throw new OutOfZoneException("Trip is out of zone.");

            trip.ZoneId = pickupZone.Id;
            trip.RiderId = riderId;

            (trip.DistanceKm, trip.DurationMinutes) = await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.DistinationCoordinates);
            trip.Price = _pricingService.CalculatePrice(_mapper.Map<Trip, CalculatePriceDto>(trip));

            if (dto.PaymentMethod == PaymentMethod.Wallet && !await _walletService.CheckUserBalanceAsync(riderId, (decimal)trip.Price))
                throw new WalletBalanceNotSufficantException($"Your wallet balance not suficant");

            await _tripRepo.CreateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            _tripEvents.FireTripRequested(_mapper.Map<Trip, TripDto>(trip));
        }
        public async Task ConfirmTripAsync(string riderId, Guid tripId)
        {
            _validator.ValidateConfirmTrip(riderId, tripId);

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

            _tripEvents.FireTripConfirmed(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task AcceptTripAsync(string driverId, Guid tripId)
        {
            _validator.ValidateAcceptTrip(driverId, tripId);

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

            _tripEvents.FireTripAccepted(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task StartTripAsync(string driverId, Guid tripId)
        {
            _validator.ValidateStartTrip(driverId, tripId);

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


            _tripEvents.FireTripStarted(_mapper.Map<Trip, TripDto>(trip));
        }


        public async Task EndTripAsync(string driverId, Guid tripId)
        {
            _validator.ValidateEndTrip(driverId, tripId);

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

            trip.TripStatus = TripStatus.Ended;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            if(trip.PaymentMethod == PaymentMethod.Wallet)
                await _walletService.TransferAsync(trip.RiderId, trip.DriverId, (decimal) trip.Price, $"{tripId}");

            await _driverService.SetDriverAvailableAsync(driverId, driver.Coordinates);

            _tripEvents.FireTripEnded(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task<TripPaginationDto> GetAllPaginatedAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            _validator.ValidateGetAllPaginated(orderBy, descending, pageNumber,pageSize);

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
            _validator.ValidateGetAllDriverTripsPaginated(driverId, orderBy, descending, pageNumber, pageSize);

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
            _validator.ValidateGetAllRiderTripsPaginated(riderId, orderBy, descending, pageNumber, pageSize);

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
            _validator.ValidateGetById(id);

            var trip = await _tripRepo.GetByIdAsync(id);
            if(trip == null)
                throw new NotFoundException($"Trip with ID '{id}' was not found.");

            return _mapper.Map<Trip, TripDto>(trip);
        }

        public async Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(Guid zoneId)
        {
            _validator.ValidateGetByRequestedTripsByZone(zoneId);
            return _mapper.Map<IEnumerable<Trip>, IEnumerable<TripDto>>( await _tripRepo.GetAvailableTripsByZoneAsync(zoneId));
        }

        public async Task<int> GetPagesCountAsync(int pageSize = 10)
        {
            _validator.ValidateGetPagesCount(pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetCountAsync()) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetRiderTripsPagesCountAsync(string riderId, int pageSize = 10)
        {
            _validator.ValidateGetRiderTripsPagesCount(riderId, pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetRiderTripsCountAsync(riderId)) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetDriverTripsPagesCountAsync(string driverId, int pageSize = 10)
        {
            _validator.ValidateGetDriverTripsPagesCount(driverId, pageSize);

            double noPages = Math.Ceiling((double)(await _tripRepo.GetDriverTripsCountAsync(driverId)) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<TripDto?> GetDriverActiveTripAsync(string driverId)
        {
            _validator.ValidateGetDriverActiveTrip(driverId);

            return _mapper.Map<Trip?, TripDto?>(await _tripRepo.GetDriverActiveTripAsync(driverId));
        }

        public async Task<TripDto?> GetRiderActiveTripAsync(string riderId)
        {
            _validator.ValidateGetRiderActiveTrip(riderId);

            return _mapper.Map<Trip?, TripDto?>(await _tripRepo.GetRiderActiveTripAsync(riderId));
        }

        public async Task UpdateTripLocationAsync(Guid tripId, Coordinates coordinates)
        {
            _validator.ValidateUpdateTripLocation(tripId, coordinates);

            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            if (trip.TripStatus != TripStatus.Started)
                throw new InvalidOperationException($"Trip location cannot be updated.");

            trip.CurrentCoordinates = coordinates;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();
        }

		public async Task CancelTripAsync(string UserId, Guid tripId)
		{
            _validator.ValidateCancelTripAsync(UserId, tripId);
			var trip = await _tripRepo.GetByIdAsync(tripId);
			if (trip == null)
				throw new NotFoundException($"Trip with ID '{tripId}' was not found.");
            
            if(trip.TripStatus == TripStatus.Ended || trip.TripStatus == TripStatus.Cancelled)
            {
                throw new InvalidOperationException("Trip is already Ended or Cancelled");
            }
            if(trip.RiderId == UserId)
            {
                if(trip.TripStatus == TripStatus.Accepted)
                {
                    await _walletService.WithdrawFromWalletAsync(UserId, (decimal)(trip.Price * 0.2), $"Cancellation fee for trip {trip.Id}");

				}
                else if(trip.TripStatus == TripStatus.Started)
				{
					(double distanceKm, double durationMinutes) =  await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.CurrentCoordinates);
                    double price = _pricingService.CalculatePrice(new CalculatePriceDto
                    {
                        DistanceKm = distanceKm,
                        DurationMinutes = durationMinutes
                    });
					await _walletService.WithdrawFromWalletAsync(UserId, (decimal)price, $"Cancellation fee for trip {trip.Id}");
				}
				trip.TripStatus = TripStatus.Cancelled;
				await _tripRepo.UpdateTripAsync(trip);
				await _tripRepo.SaveChangesAsync();

                 _tripEvents.FireTripCanceled(_mapper.Map<Trip, TripDto>(trip));
            }
            else if(trip.DriverId == UserId)
            {
                if (trip.TripStatus == TripStatus.Started)
                {
                    (double distanceKm, double durationMinutes) = await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.CurrentCoordinates);
                    double price = _pricingService.CalculatePrice(new CalculatePriceDto
                    {
                        DistanceKm = distanceKm,
                        DurationMinutes = durationMinutes
                    });
					await _walletService.TransferAsync(trip.RiderId, trip.DriverId, (decimal)trip.Price, $"{tripId}");// discuss
                    await _walletService.WithdrawFromWalletAsync(UserId, (decimal)(trip.Price * 0.1), $"Cancellation fee for driver {UserId}");
				}
             	trip.TripStatus = TripStatus.Cancelled;
				await _tripRepo.UpdateTripAsync(trip);
				await _tripRepo.SaveChangesAsync();
				_tripEvents.FireTripCanceled(_mapper.Map<Trip, TripDto>(trip));
				await _driverService.SetDriverAvailableAsync(UserId, trip.CurrentCoordinates);
			}
            else
            {
				throw new InvalidOperationException($"Id did not match wiht any rider or driver");
			}
		}
	}
}
