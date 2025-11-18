using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Validators
{
    public class DriverServiceValidator
    {
        public void ValidateUpdateLocation(string driverId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public void ValidateSetDriverAvailable(string driverId, Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
        public void ValidateSetDriverOffline(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }

        public void ValidateSetDriverInTrip(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException(driverId);
        }
    
        public void ValidateGetAvailableDriversByZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }
        public void ValidateGetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(id);
        }
    }
}
