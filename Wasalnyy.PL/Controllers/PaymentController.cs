using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.V2;
using Wasalnyy.BLL.DTO.Payment;
using Wasalnyy.BLL.DTO.Wallet;
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
			var successUrl = $"http://localhost:4200/payment-successful?amount={amount}&session_id={{CHECKOUT_SESSION_ID}}";
			var cancelUrl = "http://localhost:4200/payment-failed";
			var sessionUrl = await _paymentService.CreatePaymentSession(amount, "usd", successUrl, cancelUrl);
			return Ok(new { url = sessionUrl });
		}

        [HttpPost("handle-rider-payment")]
        [Authorize(Roles = "Rider")]

        public async Task<IActionResult> HandleRiderPayment([FromBody] HandlePaymentDTO handlePaymentDTO)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            if (handlePaymentDTO == null)
                return BadRequest("Payment result is required.");

            if (handlePaymentDTO.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            if (string.IsNullOrWhiteSpace(handlePaymentDTO.TransactionId))
                return BadRequest("TransactionId is required.");


            if (handlePaymentDTO.Status != PaymentStatus.Success &&
                handlePaymentDTO.Status != PaymentStatus.Fail)
            {
                return BadRequest("Payment status must be either (1) for Success or (0) for Fail.");
            }

            // Call business layer
            var result = await _paymentService.HandleRiderPayment(
                new RiderPaymentDetailsDTO
                {
                    RiderId = riderId,
                    TransactionId = handlePaymentDTO.TransactionId,
                    Status = handlePaymentDTO.Status,
                    Amount = handlePaymentDTO.Amount
                });
            if (!result.IsSuccess) // <-- check the service response
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);

            return Ok(new { message = result.Message });
        }




    }
}
