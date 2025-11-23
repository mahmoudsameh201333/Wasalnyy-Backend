using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Payment;

namespace Wasalnyy.BLL.Service.Abstraction
{
	public interface IPaymentService
	{
		Task<string> CreatePaymentSession(decimal amount, string currency, string successUrl, string cancelUrl);
        Task<RiderPaymentSuccessResponse> HandleRiderPayment(RiderPaymentDetailsDTO paymentResult);
    }
}
