using EventListenerSink;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
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
                services.AddHostedService<DispatcherService>();
                services.AddSingleton(new BlockingCollection<Tuple<string,string>>());
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
