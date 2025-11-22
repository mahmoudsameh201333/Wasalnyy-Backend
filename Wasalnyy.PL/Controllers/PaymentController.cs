using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.DTO.Payment;
using Wasalnyy.BLL.Service.Implementation;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;

		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpPost("create-session")]
		public async Task<IActionResult> CreateSession([FromBody] decimal amount)
		{
			var successUrl = "http://localhost:4200/choose-user-type";
			var cancelUrl = "http://localhost:4200/driver-dashboard";
			var sessionUrl = await _paymentService.CreatePaymentSession(amount, "usd", successUrl, cancelUrl);
			return Ok(new { url = sessionUrl });
		}

        [HttpPost("handle-rider-payment")]
        public async Task<IActionResult> HandleRiderPayment([FromBody] RiderPaymentDetailsDTO paymentDetails)
        {
            if (paymentDetails == null)
                return BadRequest("Payment result is required.");

            if (paymentDetails.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            if (string.IsNullOrWhiteSpace(paymentDetails.RiderId))
                return BadRequest("RiderId is required.");

            if (string.IsNullOrWhiteSpace(paymentDetails.TransactionId))
                return BadRequest("TransactionId is required.");
            if (paymentDetails.Status != PaymentStatus.Success &&
                paymentDetails.Status != PaymentStatus.Fail)
            {
                return BadRequest("Payment status must be either (1) for Success or (0) for Fail.");
            }

            // Call business layer
            var result = await _paymentService.HandleRiderPayment(paymentDetails);

            if (!result.IsSuccess) // <-- check the service response
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);

            return Ok(new { message = result.Message });
        }




    }
}
