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
         Task<int> GetCountAsync();
        Task UpdateRiderAsync(Rider rider);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
