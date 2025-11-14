using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IRiderService
    {
        Task<ReturnDriverDto?> GetByIdAsync(string id);
    }
}
