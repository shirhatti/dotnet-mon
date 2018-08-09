using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace EventListenerSink
{
    public class SimpleEventListener : EventListener
    {
        private IHubContext<DiagnosticsHub> _hubContext;
        public int EventCount { get; private set; } = 0;

        public SimpleEventListener(IHubContext<DiagnosticsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Enable runtime event source
            if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
            {
                //EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)(0x4c14fccbd));
            }
            if (eventSource.Name.Equals("Microsoft-AspNetCore-Server-Kestrel"))
            {
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)(-1));
            }
        }
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            _hubContext.Clients.All.SendAsync("Send", $"\nID = {eventData.EventId} Name = {eventData.EventName}");
            //Console.WriteLine($"\nID = {eventData.EventId} Name = {eventData.EventName}");
            for (int i = 0; i < eventData.Payload.Count; i++)
            {
                string payloadString = eventData.Payload[i] != null ? eventData.Payload[i].ToString() : string.Empty;
                //Console.WriteLine($"\tName = \"{eventData.PayloadNames[i]}\" Value = \"{payloadString}\"");
                _hubContext.Clients.All.SendAsync("Send", $"\tName = \"{eventData.PayloadNames[i]}\" Value = \"{payloadString}\"");
            }
            EventCount++;
        }
    }
}
