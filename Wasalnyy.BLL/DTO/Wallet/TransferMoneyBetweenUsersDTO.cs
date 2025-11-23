using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class TransferMoneyBetweenUsersDTO
    {
        public string DriverId { get; set; }
        public string RiderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid TripId { get; set; }
     
    }
}
