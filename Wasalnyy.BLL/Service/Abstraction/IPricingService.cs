using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Pricing;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IPricingService
    {
        public double CalculatePrice(CalculatePriceDto dto);
    }
}
