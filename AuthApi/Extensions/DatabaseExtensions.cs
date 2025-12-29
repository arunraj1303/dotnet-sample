using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Configure MySQL/MariaDB Database with connection string from environment variables
        /// </summary>
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
        {
            // Build connection string from environment variables
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

            var connectionString = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPassword}";

            // MySQL/MariaDB DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            });

            return services;
        }
    }
}
