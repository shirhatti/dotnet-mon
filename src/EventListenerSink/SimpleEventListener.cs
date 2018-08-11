using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace EventListenerSink
{
    public class SimpleEventListener : EventListener
    {
        private readonly BlockingCollection<Tuple<string, string>> _eventCollection;

        public SimpleEventListener(BlockingCollection<Tuple<string,string>> eventCollection)
        {
            _eventCollection = eventCollection;
        }
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Enable runtime event source
            if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
            {
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)(0x4c14fccbd));
            }
            if (eventSource.Name.Equals("Microsoft-AspNetCore-Server-Kestrel"))
            {
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)(-1));
            }
        }
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName.Equals("ContentionStart_V1"))
                return;
            _eventCollection?.Add(new Tuple<string, string>(eventData.EventSource.Name, eventData.EventName));
            //Console.WriteLine($"\nID = {eventData.EventId} Name = {eventData.EventName}");
            //Console.WriteLine($"\tName = \"Event Keyword\" Value = \"{(int)(eventData.Keywords)}\"");
            //for (int i = 0; i < eventData.Payload.Count; i++)
            //{
            //    string payloadString = eventData.Payload[i] != null ? eventData.Payload[i].ToString() : string.Empty;
            //    Console.WriteLine($"\tName = \"{eventData.PayloadNames[i]}\" Value = \"{payloadString}\"");
            //}
        }
    }
}
