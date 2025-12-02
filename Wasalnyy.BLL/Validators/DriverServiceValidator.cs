namespace Wasalnyy.BLL.Validators
{
    public class DriverServiceValidator
    {
        public async Task ValidateUpdateLocation(string driverId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public async Task ValidateSetDriverAvailable(string driverId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public async Task ValidateSetDriverOffline(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }

        public async Task ValidateSetDriverInTrip(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
    
        public async Task ValidateGetAvailableDriversByZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }
        public async Task ValidateGetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(id);
        }
    }
}
