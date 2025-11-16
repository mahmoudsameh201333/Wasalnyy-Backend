
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Wasalnyy.BLL.Common;
using Wasalnyy.BLL.Common;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.Settings;
using Wasalnyy.DAL.Common;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.PL.EventHandlers.Implementation;
using Wasalnyy.PL.Hubs;
namespace Wasalnyy.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500", "http://localhost:4200") // frontend URLs
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSignalR();

			builder.Services.AddDbContext<WasalnyyDbContext>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.Configure<PricingSettings>(builder.Configuration.GetSection("PricingSettings"));
            builder.Services.AddScoped<PricingSettings>(sp =>
                sp.GetRequiredService<IOptions<PricingSettings>>().Value);


            builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                           .AddEntityFrameworkStores<WasalnyyDbContext>()
                           .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IDriverNotifier, DriverNotifier>();
            builder.Services.AddSingleton<IRiderNotifier, RiderNotifier>();
            builder.Services.AddSingleton<ITripNotifier, TripNotifier>();


            builder.Services.AddBussinessInPL(builder.Configuration);
            builder.Services.AddBussinessInDAL();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.UseBussinessEventSubscriptions();

            app.UseCors("AllowAll");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var userManager = services.GetRequiredService<UserManager<User>>();
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DbSeeder.SeedAsync(userManager, roleManager);
                
                var connectionService = services.GetRequiredService<IWasalnyyHubConnectionService>();
                await connectionService.DeleteAllConnectionsAsync();
            }
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapHub<WasalnyyHub>("/Wasalnyy");

            app.MapControllers();
            app.Run();
        }
    }
}
