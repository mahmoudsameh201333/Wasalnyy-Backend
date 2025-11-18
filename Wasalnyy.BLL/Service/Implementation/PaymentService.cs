using Stripe;
using Stripe.Checkout;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration config;

		public PaymentService(IConfiguration config)
		{
			this.config = config;
			StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
		}
		public async Task<string> CreatePaymentSession(decimal amount, string currency, string successUrl, string cancelUrl)
		{
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = new List<SessionLineItemOptions>
			{
				new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(amount * 100), // amount in cents
                        Currency = currency,
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = "Ride Payment",
						},
					},
					Quantity = 1,
				},
			},
				Mode = "payment",
				SuccessUrl = successUrl,
				CancelUrl = cancelUrl
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);
			return session.Url;
		}
	}
}
