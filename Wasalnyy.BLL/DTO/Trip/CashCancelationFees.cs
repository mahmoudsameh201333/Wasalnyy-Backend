using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Trip
{
    public class CashCancelationFees
    {
        public double DistanceTraveledCost { get; set; }
        public double Fees { get; set; }
        public double Total { get => DistanceTraveledCost + Fees; }

    }
}
