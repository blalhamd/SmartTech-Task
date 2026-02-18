using Serilog;

namespace EduNexus.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilogConfiguration(this IHostBuilder host)
        {
            host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }
    }
}
