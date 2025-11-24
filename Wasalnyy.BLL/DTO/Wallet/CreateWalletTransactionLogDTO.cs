using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
<<<<<<<< HEAD:Wasalnyy.DAL/Entities/WalletTransaction.cs
    public class WalletTransaction
========
    public class CreateWalletTransactionLogDTO
>>>>>>>> 6f7b02c18388453ec0ef2c3ec5e3ba9ae73364cc:Wasalnyy.BLL/DTO/Wallet/CreateWalletTransactionLogDTO.cs
    {
        public Guid Id { get; set; }

        // FK → Wallet
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }

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
