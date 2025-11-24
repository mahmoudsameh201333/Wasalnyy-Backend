using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Wallet
{
    public class AddWalletTransactionDto
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "Credit" or "Debit"
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
