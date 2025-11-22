

namespace Wasalnyy.DAL.Entities
{
    public class WalletTransaction
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
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
