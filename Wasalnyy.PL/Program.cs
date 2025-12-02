using FaceRecognitionDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wasalnyy.BLL.Common;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.BLL.Settings;
using Wasalnyy.DAL.Common;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.PL.EventHandlers.Implementation;
using Wasalnyy.PL.Filters;
using Wasalnyy.PL.Hubs;
using Wasalnyy.PL.Middleware;

namespace Wasalnyy.PL
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			//builder.Services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowAll", policy =>
			//	{
			//		policy.AllowAnyOrigin()
			//			  .AllowAnyMethod()
			//			  .AllowAnyHeader();
			//	});
			//});

			// Add services to the container.
			builder.Services.AddControllers();

			builder.Services.AddSignalR(options =>
			{
				options.AddFilter<SignalRExceptionFilter>();
			});

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

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					policy => policy.WithOrigins("http://localhost:4200")
									.AllowAnyHeader()
									.AllowAnyMethod().
									AllowCredentials());
			});
			var modelPath = Path.Combine(builder.Environment.ContentRootPath, "models");
			Console.WriteLine($"Loading face models from: {modelPath}");
			builder.Services.AddSingleton(sp =>
			{
				return FaceRecognition.Create(modelPath);
			});
			builder.Services.AddBussinessInPL(builder.Configuration);
			builder.Services.AddBussinessInDAL();
			builder.Services.AddHttpClient();
			builder.Services.AddSingleton<IDriverNotifier, DriverNotifier>();
			builder.Services.AddSingleton<ITripNotifier, TripNotifier>();
			builder.Services.AddSingleton<IWasalnyyHubNotifier, WasalnyyHubNotifier>();

			builder.Services.AddScoped<WasalnyyOnlineActionFilter>();

			builder.Services.AddHttpClient();

			var app = builder.Build();

			app.UseBussinessEventSubscriptions();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var userManager = services.GetRequiredService<UserManager<User>>();
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

				await DbSeeder.SeedAsync(userManager, roleManager);

				var connectionService = services.GetRequiredService<IWasalnyyHubService>();
				await connectionService.DeleteAllConnectionsAsync();
			}
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseStaticFiles();
			app.UseMiddleware<ExptionhandlingMiddleware>();
            app.UseWebSockets();
            app.MapHub<WasalnyyHub>("/Wasalnyy");
			app.MapControllers();
			app.Run();
		}
	}
}
