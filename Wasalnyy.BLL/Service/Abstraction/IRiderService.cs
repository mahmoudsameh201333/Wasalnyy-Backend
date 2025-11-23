using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Rider;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IRiderService
    {
        Task<ReturnRiderDto?> GetByIdAsync(string id);
        Task<bool> UpdateRiderInfo(string id, RiderUpdateDto riderUpdate);
        //rider infos in home page
        Task<string> RiderName(string id); 
        Task<string?> RiderProfileImage(string id); 
        Task<int> RiderTotalTrips(string id); 
        Task<bool> IsRiderSuspended(string id); 
        Task<decimal> RiderWalletBalance(string id); // wallet amount
    }
}
