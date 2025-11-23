using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wasalnyy.BLL.Enents;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.Mapper;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Service.Implementation;
using Wasalnyy.BLL.Settings;
using Wasalnyy.BLL.Validators;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Common
{
    public static class ModularBussinessLogicLayer
    {
        public static IServiceCollection AddBussinessInPL(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(x => x.AddProfile(new DomainProfile()));

            //Payment 
            
            services.AddScoped<IPaymentGetwayRepo, PaymentGetwayRepo>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletTransactionRepo, WalletTransactionRepo>(); 

            // Register services
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IRiderService, RiderService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IWasalnyyHubService, WasalnyyHubService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPasswordService, PasswordService>();

            services.AddScoped<DriverServiceValidator>();
            services.AddScoped<TripServiceValidator>();


            // Register events and notifiers
            services.AddSingleton<DriverEvents>();
            services.AddSingleton<RiderEvents>();
            services.AddSingleton<TripEvents>();
            services.AddSingleton<WasalnyyHubEvents>();
            services.AddSingleton<IFaceService, FaceService>();


			services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
			var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
			var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
			});


            services.Configure<PricingSettings>(configuration.GetSection("PricingSettings"));
            services.AddScoped<PricingSettings>(sp =>
                sp.GetRequiredService<IOptions<PricingSettings>>().Value);


            services.AddScoped<JwtHandler>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IEmailService, EmailService>();
			return services;
        }
        public static IApplicationBuilder UseBussinessEventSubscriptions(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var tripEvents = scope.ServiceProvider.GetRequiredService<TripEvents>();
            var driverEvents = scope.ServiceProvider.GetRequiredService<DriverEvents>();
            var wasalnyyHubEvents = scope.ServiceProvider.GetRequiredService<WasalnyyHubEvents>();

            var tripHandler = scope.ServiceProvider.GetRequiredService<ITripNotifier>();
            var driverHandler = scope.ServiceProvider.GetRequiredService<IDriverNotifier>();
            var wasalnyyHubHandler = scope.ServiceProvider.GetRequiredService<IWasalnyyHubNotifier>();

            tripEvents.TripRequested += tripHandler.OnTripRequested;
            tripEvents.TripAccepted += tripHandler.OnTripAccepted;
            tripEvents.TripStarted += tripHandler.OnTripStarted;
            tripEvents.TripEnded += tripHandler.OnTripEnded;
            tripEvents.TripCanceled += tripHandler.OnTripCanceled;




            driverEvents.DriverStatusChangedToAvailable += driverHandler.OnDriverStatusChangedToAvailable;
            driverEvents.DriverZoneChanged += driverHandler.OnDriverZoneChanged;
            driverEvents.DriverLocationUpdated += driverHandler.OnDriverLocationUpdated;
            driverEvents.DriverOutOfZone += driverHandler.OnDriverOutOfZone;
            driverEvents.DriverStatusChangedToUnAvailable += driverHandler.OnDriverStatusChangedToUnAvailable;
            //driverEvents.DriverStatusChangedToInTrip += driverHandler.OnDriverStatusChangedToInTrip;


            wasalnyyHubEvents.UserConnected += wasalnyyHubHandler.OnUserConnected;
            wasalnyyHubEvents.UserDisconnected += wasalnyyHubHandler.OnUserDisconnected;

            return app;
        }
    }
}
