using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IRiderRepo
    {
        Task<Rider?> GetByIdAsync(string driverId);
        Task<IEnumerable<Rider>> GetAllRidersAsync();

        Task<Rider?> GetByPhoneAsync(string phonenum);
         Task<double> GetCountAsync();
        Task UpdateRiderAsync(Rider rider);
        Task<IEnumerable<Trip>> GetRiderTripsByPhone(string phone);
        public Task<IEnumerable<Complaint>> GetRiderComplainsByPhone(string phone);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
