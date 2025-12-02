namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IDriverRepo
    {
        Task ChangeStatusAsync(string driverId, DriverStatus status);
        Task UpdateAsync(Driver driver);
        Task<Driver?> GetByIdAsync(string driverId);
        Task<IEnumerable<Driver>> GetAvailableDriversByZoneAsync(Guid zoneId);
        Task UpdateDriverZoneAsync(string driverId, Guid zoneId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Driver>> GetAllDriverAsync();
        Task<Driver> GetDriverByLicense(string licen);
        Task<int> GetCountAsync();
    }
}
