using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventListenerSink
{
    public class NamedPipeSink
    {
        private CancellationTokenSource _cts;
        private object _internalLock;
        private NamedPipeServerStream PipeStream { get; set; }

        public NamedPipeSink(string pipeName)
        {
            _internalLock = new object();
            PipeStream = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
        }
        public void Dispose()
        {
            // TODO need to make sure we're stopped before we dispose
            PipeStream.Dispose();
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // Create listener thread
            Task.Run(async () =>
            {
                await ListenAsync(token);
            }, token);
        }

        private async Task ListenAsync(CancellationToken ct)
        {
            await PipeStream.WaitForConnectionAsync(ct);
            Console.WriteLine("dotnet-mon connected");

            while (true)
            {
                try
                {
                    
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                    Console.WriteLine("Aborting listener thread");
                    break;
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public void SendMessage(string message)
        {
            if (!PipeStream.IsConnected)
            {
                return;
            }
            var buffer = Encoding.Unicode.GetBytes(message);
            lock (_internalLock)
            {
                PipeStream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
