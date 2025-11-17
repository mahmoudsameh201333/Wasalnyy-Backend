using FaceRecognitionDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wasalnyy.BLL.Common;
using Wasalnyy.BLL.Service.Implementation;
using Wasalnyy.BLL.Settings;
using Wasalnyy.DAL.Common;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
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
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();
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

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAngular",
					policy => policy.WithOrigins("http://localhost:4200")
									.AllowAnyHeader()
									.AllowAnyMethod());
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

			var app = builder.Build();

			app.UseBussinessEventSubscriptions();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var userManager = services.GetRequiredService<UserManager<User>>();
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

				await DbSeeder.SeedAsync(userManager, roleManager);
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
			app.UseCors("AllowAngular");
			app.MapHub<WasalnyyHub>("/Wasalnyy");
			app.MapControllers();
			app.Run();
		}
	}
}
