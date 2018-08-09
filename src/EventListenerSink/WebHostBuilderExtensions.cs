using EventListenerSink;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseDotNetMon(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<SimpleEventListener>();
                services.AddSingleton<IStartupFilter>(new DiagnosticsStartupFilter());
                services.AddSignalR().AddMessagePackProtocol();
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.Listen(IPAddress.Any, 5000);
                    options.Listen(IPAddress.Any, 9001, builder =>
                    {
                        builder.UseHub<DiagnosticsHub>();
                    });
                });
            });
        }
    }
}
