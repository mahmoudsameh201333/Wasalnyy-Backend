using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Pricing;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Settings;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class PricingService : IPricingService
    {
        private readonly PricingSettings _settings;

        public PricingService(PricingSettings settings)
        {
            _settings = settings;
        }


        public double CalculatePrice(CalculatePriceDto dto)
        {
            var total = _settings.BaseFare
                                    + (dto.DistanceKm * _settings.PricePerKm)
                                    + (dto.DurationMinutes * _settings.PricePerMinute);

            return Math.Round(total * _settings.SurgeMultiplier, 2);
        }
    }
}
