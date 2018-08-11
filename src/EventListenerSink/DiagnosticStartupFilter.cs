using EventListenerSink;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.Tracing;

namespace Microsoft.AspNetCore.Hosting
{
    public class DiagnosticsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                var runtimeEventSource = RuntimeEventSource.Log;
                var eventListener = app.ApplicationServices.GetService<SimpleEventListener>();
                //eventListener.EnableEvents(runtimeEventSource, EventLevel.Verbose, (EventKeywords)(0x4c14fccbd));
                next(app);
            };
        }
    }
}