using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ServiceBus.Messaging;

namespace EvilCorp.SignalHub.TrackerEvents
{
    public class EventHubTrackerSource
    {
        public EventHubTrackerSource(Action<string> onReceive)
        {
            this._onReceive = onReceive;
        }

        public async Task StartAsync()
        {
            string eventHubConnectionString = "Endpoint=sb://relay-dev.servicebus.windows.net/;SharedAccessKeyName=ReceiveRule;SharedAccessKey=FBJkNG0oPTYQW2eGnd4LQJSFtpneYZpFB4jT+2136bw=";
            string eventHubName = "evil-hub";
            string storageAccountName = "evilstorage";
            string storageAccountKey = "UAX64PuM4JC3BPRMQ7bcDbfs3yXq6TlEceG6v1IYK3inAemzMqwP3OPwEF1XtDtk4xNDem2NWqgZVsQ6gzXK6Q==";
            string storageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);
            string eventProcessorHostName = Guid.NewGuid().ToString();

            var factory = new SimpleEventProcessorFactory(this._onReceive);

            await EventHubTrackerSource.AttachProcessorForHub(eventProcessorHostName, eventHubConnectionString, storageConnectionString, eventHubName, factory);
        }

        private Action<string> _onReceive;


        public static async Task<EventProcessorHost> AttachProcessorForHub(
            string processorName,
            string serviceBusConnectionString,
            string offsetStorageConnectionString,
            string eventHubName,
            IEventProcessorFactory processorFactory)
        {
            var eventProcessorHost = new EventProcessorHost(processorName, eventHubName, EventHubConsumerGroup.DefaultGroupName, serviceBusConnectionString, offsetStorageConnectionString);
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(processorFactory);

            return eventProcessorHost;
        }
    }


}