using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class AddWalletTranferMoneyDTO
    {

        // Wallet → Sender
        public Guid SenderWalletId { get; set; }

        // Wallet → Receiver
        public Guid ReceiverWalletId { get; set; }

        public decimal Amount { get; set; }

        // Optional: Trip link if the transfer is related to a ride
        public Guid? TripId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
