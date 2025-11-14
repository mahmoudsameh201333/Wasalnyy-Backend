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
        private readonly IMapper _mapper;

        public TripService(ITripRepo tripRepo, IDriverService driverService, TripEvents tripEvents,
            IRiderService riderService, IRouteService routeService, IPricingService pricingService,
            IZoneService zoneService, IMapper mapper)
        {
            _tripRepo = tripRepo;
            _driverService = driverService;
            _tripEvents = tripEvents;
            _riderService = riderService;
            _routeService = routeService;
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async Task RequestTripAsync(string riderId, RequestTripDto dto)
        {
            var rider = await _riderService.GetByIdAsync(riderId);
            if(rider == null)
                throw new NotFoundException($"Rider with ID '{riderId}' was not found.");

            var trip = _mapper.Map<RequestTripDto, Trip>(dto);

            var zone = await _zoneService.GetZoneAsync(trip.PickupCoordinates);

            if (zone == null)
                throw new OutOfZoneException("Trip is out of zone.");

            trip.ZoneId = zone.Id;


            trip.RiderId = riderId;

            (trip.DistanceKm, trip.DurationMinutes) = await _routeService.CalculateDistanceAndDurationAsync(trip.PickupCoordinates, trip.DistinationCoordinates);
            trip.Price = _pricingService.CalculatePrice(_mapper.Map<Trip, CalculatePriceDto>(trip));

            await _tripRepo.CreateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            _tripEvents.FireTripRequested(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task AcceptTripAsync(string driverId, Guid tripId)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            trip.DriverId = driverId;
            trip.TripStatus = TripStatus.Accepted;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _driverService.SetDriverInTripAsync(driverId, tripId);

            _tripEvents.FireTripAccepted(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task StartTripAsyncAsync(string driverId, Guid tripId)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if(trip.DriverId !=  driverId)
                throw new DriverMismatchException($"Driver '{driverId}' is not assigned to trip '{tripId}'.");

            trip.TripStatus = TripStatus.Started;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();


            _tripEvents.FireTripStarted(_mapper.Map<Trip, TripDto>(trip));
        }


        public async Task EndTripAsync(string driverId, Guid tripId)
        {
            var trip = await _tripRepo.GetByIdAsync(tripId);
            if (trip == null)
                throw new NotFoundException($"Trip with ID '{tripId}' was not found.");

            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if (trip.DriverId != driverId)
                throw new DriverMismatchException($"Driver '{driverId}' is not assigned to trip '{tripId}'.");

            trip.TripStatus = TripStatus.Ended;

            await _tripRepo.UpdateTripAsync(trip);
            await _tripRepo.SaveChangesAsync();

            await _driverService.SetDriverAvailableAsync(driverId);

            _tripEvents.FireTripEnded(_mapper.Map<Trip, TripDto>(trip));
        }

        public async Task<TripPaginationDto> GetAllPaginatedAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
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
            var trip = await _tripRepo.GetByIdAsync(id);
            if(trip == null)
                throw new NotFoundException($"Trip with ID '{id}' was not found.");

            return _mapper.Map<Trip, TripDto>(trip);
        }

        public async Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(Guid zoneId)
        {
            return _mapper.Map<IEnumerable<Trip>, IEnumerable<TripDto>>( await _tripRepo.GetRequestedTripsByZoneAsync(zoneId));
        }

        public async Task<int> GetPagesCountAsync(int pageSize = 10)
        {
            double noPages = Math.Ceiling((double)(await _tripRepo.GetCountAsync()) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetRiderTripsPagesCountAsync(string riderId, int pageSize = 10)
        {
            double noPages = Math.Ceiling((double)(await _tripRepo.GetRiderTripsCountAsync(riderId)) / (double)pageSize);

            return (int)noPages;
        }

        public async Task<int> GetDriverTripsPagesCountAsync(string driverId, int pageSize = 10)
        {
            double noPages = Math.Ceiling((double)(await _tripRepo.GetDriverTripsCountAsync(driverId)) / (double)pageSize);

            return (int)noPages;
        }
    }
}
