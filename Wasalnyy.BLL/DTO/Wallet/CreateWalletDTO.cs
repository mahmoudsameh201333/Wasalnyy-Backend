using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class CreateWalletDTO
    {

        // Link to User (Rider or Driver)
        public string UserId { get; set; }

        // Current balance
        public decimal Balance { get; set; } = 0;

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
