using Serilog;
using Serilog.Events;
using Stratis.Bitcoin.Builder;

namespace Stratis.IndexerD
{
    public static class FullNodeBuilderExtensions
    {
        public static IFullNodeBuilder UseSerilog(this IFullNodeBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();
            builder.NodeSettings.LoggerFactory.AddSerilog();
            return builder;
        }
    }
}
