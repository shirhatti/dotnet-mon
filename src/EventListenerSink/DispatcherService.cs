using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventListenerSink
{
    public class DispatcherService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IHubContext<DiagnosticsHub> _hub;
        private Timer _timer;
        private BlockingCollection<Tuple<string,string>> _eventCollection;

        public DispatcherService(ILogger<DispatcherService> logger, IHubContext<DiagnosticsHub> hub, BlockingCollection<Tuple<string, string>> eventCollection)
        {
            _logger = logger;
            _hub = hub;
            _eventCollection = eventCollection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DispatcherService is starting.");
            _timer = new Timer(Dispatch, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DispatcherService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Dispatch(object state)
        {
            while (!_eventCollection.IsCompleted)
            {
                Tuple<string, string> datum;
                try
                {
                    datum = _eventCollection.Take();
                    if (datum != null)
                    {
                        _hub.Clients.All.SendAsync("Send", datum.Item1 + datum.Item2);
                    }
                }
                catch (InvalidOperationException) { }
            }
        }
    }
}
