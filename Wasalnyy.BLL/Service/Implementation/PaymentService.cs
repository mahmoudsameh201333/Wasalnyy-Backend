using AutoMapper;
using Stripe;
using Stripe.Checkout;
using Wasalnyy.BLL.DTO.Payment;
using Wasalnyy.BLL.DTO.Wallet;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration config;
		private readonly IPaymentGetwayRepo paymentGetwayRepo;
        private readonly IMapper _mapper;
		private readonly IWalletService	walletService;
        public PaymentService(IConfiguration config, IPaymentGetwayRepo  paymentGetwayRepo, IMapper mapper, IWalletService walletService)
		{
			this.config = config;
			StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
			this.paymentGetwayRepo = paymentGetwayRepo;
            this._mapper= mapper;

			this.walletService = walletService;
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

        public async Task<RiderPaymentSuccessResponse> HandleRiderPayment(RiderPaymentDetailsDTO paymentDetails)
        {
            if (paymentDetails.Status == PaymentStatus.Success)
                return await HandleSuccessfulPaymentAsync(paymentDetails);
            else
                return await HandleFailedPaymentAsync(paymentDetails);
        }
        private async Task<RiderPaymentSuccessResponse> HandleSuccessfulPaymentAsync(RiderPaymentDetailsDTO paymentDetails)
        {
            var paymentEntity = _mapper.Map<GatewayPayment>(paymentDetails);
            //1- save payment transaction in payment getway table as successful
            try
            {
                await paymentGetwayRepo.AddPaymentAsync(paymentEntity);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new RiderPaymentSuccessResponse(false, $"Error saving payment: {innerMessage}");
            }

            //2-Increase Wallet balance 

            var walletResponse = await walletService.IncreaseWalletAsync(paymentDetails.RiderId, paymentDetails.Amount);
            if (!walletResponse.IsSuccess)
                return new RiderPaymentSuccessResponse(false, $"Error updating wallet: {walletResponse.Message}");

            return new RiderPaymentSuccessResponse(true, "Payment processed successfully.");
        }

        private async Task<RiderPaymentSuccessResponse> HandleFailedPaymentAsync(RiderPaymentDetailsDTO paymentDetails)
        {
            var paymentEntity = _mapper.Map<GatewayPayment>(paymentDetails);
            //save payment transaction in payment getway table as failed
            try
            {
                await paymentGetwayRepo.AddPaymentAsync(paymentEntity);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new RiderPaymentSuccessResponse(false, $"Error saving payment: {innerMessage}");
            }


            return new RiderPaymentSuccessResponse(true, "Payment failer is saved in Db successfully.");
        }
    }
}
