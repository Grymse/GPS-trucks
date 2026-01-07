using GPSTrucks.Simulator.Application.Repositories;
using GPSTrucks.Simulator.Infrastructure.Contexts;
using GPSTrucks.Simulator.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GPSTrucks.Simulator.Infrastructure
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LoggingDatabase");
            services.AddDbContext<LoggingDbContext>(options => options.UseSqlite(connectionString));

            services.AddHttpClient<IGPSDataPublisherRepository, GPSDataHTTPPublisherRepository>(client =>
            {
                client.BaseAddress = new Uri(configuration["GPS:BaseUrl"]);
            });
        }
    }
}
