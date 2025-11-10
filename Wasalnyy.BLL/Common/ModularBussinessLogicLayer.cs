using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wasalnyy.BLL.Enents;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.EventHandlers.Implementation;
using Wasalnyy.BLL.Mapper;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Service.Implementation;

namespace Wasalnyy.BLL.Common
﻿namespace EmployeeCrud.BLL.Common
{
    public static class ModularBussinessLogicLayer
    {
        public static IServiceCollection AddBussinessInPL(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(x => x.AddProfile(new DomainProfile()));
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IZoneService, ZoneService>();

            services.AddSingleton<DriverEvents>();
            services.AddSingleton<RiderEvents>();
            services.AddSingleton<TripEvents>();

            services.AddSingleton<IDriverNotifier, DriverNotifier>();
            services.AddSingleton<IRiderNotifier, RiderNotifier>();
            services.AddSingleton<ITripNotifier, TripNotifier>();


            return services;
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
			services.AddScoped<JwtHandler>();
			services.AddScoped<IAuthService, AuthService>();
			return services;
        }
        public static IApplicationBuilder UseBussinessEventSubscriptions(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var tripEvents = scope.ServiceProvider.GetRequiredService<TripEvents>();
            var driverEvents = scope.ServiceProvider.GetRequiredService<DriverEvents>();

            var tripHandler = scope.ServiceProvider.GetRequiredService<ITripNotifier>();
            var DriverHandler = scope.ServiceProvider.GetRequiredService<IDriverNotifier>();

            tripEvents.TripRequested += tripHandler.OnTripRequested;
            tripEvents.TripAccepted += tripHandler.OnTripAccepted;
            tripEvents.TripStarted += tripHandler.OnTripStarted;
            tripEvents.TripEnded += tripHandler.OnTripEnded;
            tripEvents.TripCanceled += tripHandler.OnTripCanceled;


            driverEvents.DriverStatusChanged += DriverHandler.OnDriverStatusChanged;
            driverEvents.DriverLocationUpdated += DriverHandler.OnDriverLocationUpdated;


            return app;
        }
    }
}
