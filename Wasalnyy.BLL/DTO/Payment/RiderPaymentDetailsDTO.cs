using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Payment
{
    public class RiderPaymentDetailsDTO
    {
        public string RiderId { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }


    }
}
