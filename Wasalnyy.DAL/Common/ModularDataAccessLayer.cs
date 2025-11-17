using Microsoft.Extensions.DependencyInjection;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.DAL.Common
{
    public static class ModularDataAccessLayer
    {
        public static IServiceCollection AddBussinessInDAL(this IServiceCollection services)
        {
            services.AddScoped<IDriverRepo, DriverRepo>();
            services.AddScoped<IRiderRepo, RiderRepo>();
            services.AddScoped<ITripRepo, TripRepo>();
            services.AddScoped<IZoneRepo, ZoneRepo>();
            services.AddScoped<IUserFaceDataRepo, UserFaceDataRepo>();
            return services;
        }
    }
}