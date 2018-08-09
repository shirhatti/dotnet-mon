using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventListenerSink
{
    public class DiagnosticsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected");
            return Task.CompletedTask;
        }
    }
}
