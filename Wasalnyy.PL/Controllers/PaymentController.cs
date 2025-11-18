using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.Service.Implementation;

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
	}
}
