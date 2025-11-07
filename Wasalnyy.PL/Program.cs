
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.PL.Hubs;

namespace Wasalnyy.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

			builder.Services.AddDbContext<WasalnyyDbContext>(options =>
	            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<WasalnyyDbContext>(options =>
            options.UseSqlServer(connectionString));
           
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
             options =>
    {
             options.LoginPath = new PathString("/Account/Login");
             options.AccessDeniedPath = new PathString("/Account/Login");
             });



            builder.Services.AddIdentityCore<WasalnyyDbContext>(options => options.SignIn.RequireConfirmedAccount = true)
                            .AddEntityFrameworkStores<WasalnyyDbContext>()
                            .AddTokenProvider<DataProtectorTokenProvider<Riders>>(TokenOptions.DefaultProvider);

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<RideHub>("/ride");


            app.MapControllers();

            app.Run();
        }
    }
}
