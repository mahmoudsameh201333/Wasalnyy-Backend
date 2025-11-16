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
    }
}
