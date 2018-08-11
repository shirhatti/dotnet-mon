using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace dotnet_mon
{
    class Program
    {
        private static readonly Table _table = new Table(150);
        public static async Task<int> Main(string[] args)
        {
            var uri = new Uri("net.tcp://127.0.0.1:9001");
            Console.WriteLine("Connecting to {0}", uri);
            var connection = new HubConnectionBuilder()
                .WithEndPoint(uri)
                .Build();

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                exitEvent.Set();
            };

            // Set up handler
            connection.On<string>("Send", DataHandler);

            CancellationTokenSource closedTokenSource = null;

            connection.Closed += e =>
            {
                // This should never be null by the time this fires
                closedTokenSource.Cancel();

                Console.WriteLine("Connection closed...");
                return Task.CompletedTask;
            };

            while(true)
            {
                if (await ConnectAsync(connection))
                {
                    break;
                }
            }

            Console.WriteLine("Listening. Press Ctrl + C to stop listening...");
            _table.Header = new Tuple<string, string>("Measure", "Value");
            _table.Start();
            exitEvent.WaitOne();
            _table.Dispose();

            return 0;
        }

        private static void DataHandler(string measure)
        {
            if (_table.Data.TryGetValue(measure, out int count))
            {
                if (!_table.Data.TryUpdate(measure, count + 1, count))
                {
                    throw new NotSupportedException();
                }
            }
            _table.Data.TryAdd(measure, 1);
        }

        private static async Task<bool> ConnectAsync(HubConnection connection)
        {
            // Keep trying to until we can start
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    return true;
                }
                catch (ObjectDisposedException)
                {
                    // Client side killed the connection
                    return false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to connect, trying again in 5000(ms)");

                    await Task.Delay(5000);
                }
            }
        }
    }
}
