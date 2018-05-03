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
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile("log-{Date}.txt")
                .WriteTo.Seq("http://52.151.72.210:5341")
                .CreateLogger();
            builder.NodeSettings.LoggerFactory.AddSerilog();
            return builder;
        }
    }
}
