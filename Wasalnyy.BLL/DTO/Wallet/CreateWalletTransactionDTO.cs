using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class CreateWalletTransactionDTO
    {

        // FK → Wallet
        public Guid WalletId { get; set; }

        // Amount involved
        public decimal Amount { get; set; }

        // Credit = add money, Debit = subtract money
        public WalletTransactionType TransactionType { get; set; }

        // Optional note (Ride #123, Stripe Payment, Withdraw, etc.)
        public string? Description { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; }
        
    }
}
