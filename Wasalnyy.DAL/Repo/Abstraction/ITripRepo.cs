using System.Linq.Expressions;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface ITripRepo
    {
        Task<Trip?> GetByIdAsync(Guid id);
        Task<Trip?> GetDriverActiveTripAsync(string driverId);
        Task<Trip?> GetRiderActiveTripAsync(string riderId);
        Task<IEnumerable<Trip>> GetAvailableTripsByZoneAsync(Guid zoneId);
        Task<IEnumerable<Trip>> GetTripsPaginatedAsync(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Trip>> GetAllRiderTripsPaginatedAsync(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<Trip>> GetAllDriverTripsPaginatedAsync(string driverId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task CreateTripAsync(Trip trip);
        Task UpdateTripAsync(Trip trip);
        Task DeleteAsync(Guid id);

        Task<int> GetCountAsync();
        Task<int> GetRiderTripsCountAsync(string riderId);
        Task<int> GetDriverTripsCountAsync(string driverId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
        Task<double> GetTripDurationAsync(Guid id);
    }
}
