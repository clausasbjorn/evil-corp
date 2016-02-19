using EvilCorp.SignalHub.Properties;
using Microsoft.AspNet.SignalR;
using System;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Threading.Tasks;
using EvilCorp.SignalHub.TrackerEvents;
using Microsoft.ServiceBus.Messaging;

namespace EvilCorp.SignalHub
{
    public class TrackerHub : Hub
    {
        private SimpleEventProcessor _processor;

      
        public TrackerHub()
        {


            _processor = new SimpleEventProcessor(data => {
                Clients.All.message(data);
            });
        }
        public static IHubContext Hub()
        {
            return GlobalHost.ConnectionManager.GetHubContext<TrackerHub>();
        }

        public static void Send(IHubContext hub, string eventData)
        {
            hub.Clients.All.seen(eventData);
        }
        
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return (base.OnReconnected());
        }
    }
}