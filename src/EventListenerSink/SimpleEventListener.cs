using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace EventListenerSink
{
    public class SimpleEventListener : EventListener
    {
        public NamedPipeSink Sink { get; private set; }
        public int EventCount { get; private set; } = 0;

        public SimpleEventListener()
        {
            //Sink = new NamedPipeSink("dotnetmon" + Process.GetCurrentProcess().Id);
            Sink = new NamedPipeSink("dotnetmon");
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
            //Console.WriteLine($"ID = {eventData.EventId} Name = {eventData.EventName}");
            //for (int i = 0; i < eventData.Payload.Count; i++)
            //{
            //    string payloadString = eventData.Payload[i] != null ? eventData.Payload[i].ToString() : string.Empty;
            //    Console.WriteLine($"\tName = \"{eventData.PayloadNames[i]}\" Value = \"{payloadString}\"");
            //}
            //Console.WriteLine("\n");
            Sink.SendMessage($"ID = {eventData.EventId} Name = {eventData.EventName}");
            EventCount++;
        }
    }
}
