

namespace Wasalnyy.DAL.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }

        // Link to User (Rider or Driver)
        public string UserId { get; set; }
        public User User { get; set; }

        // Current balance
        public decimal Balance { get; set; } = 0;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; }

        // Transaction history
        public List<WalletTransactionLogs> Transactions { get; set; } = new();
    }
}
