using AuthApi.Repositories;
using AuthApi.Services;

namespace AuthApi.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Register all application services (Business Logic Layer)
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Repository Pattern
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register Business Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICompanyService, CompanyService>();

            return services;
        }
    }
}
