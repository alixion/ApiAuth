using Serilog;
using Serilog.Core;

namespace ApiAuth.Common;

public static class ApplicationLoggerFactory
{
    public static Logger CreateLogger(IConfiguration configuration, IHostEnvironment hostEnvironment) =>
        new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
            .CreateLogger();
}