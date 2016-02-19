using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace EvilCorp.SignalHub.TrackerEvents
{
    public class SimpleEventProcessorFactory : IEventProcessorFactory
    {
        public SimpleEventProcessorFactory(Action<string> callback)
        {
            this.callback = callback;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new SimpleEventProcessor(this.callback);
        }

        private Action<string> callback;
    }

    class SimpleEventProcessor : IEventProcessor
    {
        private Action<string> _callback;

        Stopwatch checkpointStopWatch;

        public SimpleEventProcessor(Action<string> callback)
        {
            _callback = callback;
        }

        public void SetAction(Action<string> callback)
        {
            _callback = callback;
        }

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                _callback(data);
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }
    }
}