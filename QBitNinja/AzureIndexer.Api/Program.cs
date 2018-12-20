using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace AzureIndexer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logConfigurations = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .WriteTo.Trace();

            if (Directory.Exists(@"D:\home\LogFiles\Application"))
            {
                logConfigurations
                    .WriteTo.File(
                        @"D:\home\LogFiles\Application\blockexplorerapi.txt",
                        fileSizeLimitBytes: 1_000_000,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1));
            }

            Log.Logger = logConfigurations.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .ConfigureServices(services => services.AddAutofac())
                .UseIISIntegration()
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
