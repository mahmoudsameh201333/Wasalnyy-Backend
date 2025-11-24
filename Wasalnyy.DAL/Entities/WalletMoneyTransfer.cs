
namespace Wasalnyy.DAL.Entities
{
    public class WalletMoneyTransfer
    {
        public Guid Id { get; set; }

        // Wallet → Sender
        public Guid SenderWalletId { get; set; }
        public Wallet SenderWallet { get; set; }

        // Wallet → Receiver
        public Guid ReceiverWalletId { get; set; }
        public Wallet ReceiverWallet { get; set; }

        public decimal Amount { get; set; }

        // Optional: Trip link if the transfer is related to a ride
        public Guid? TripId { get; set; }
        public Trip? Trip { get; set; }

        public DateTime CreatedAt { get; set; } 
    }
}