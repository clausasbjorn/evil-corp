using EvilCorp.SignalHub.TrackerEvents;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(EvilCorp.SignalHub.Startup))]
namespace EvilCorp.SignalHub
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/signalr", new HubConfiguration()
            {
                EnableJavaScriptProxies = true,
                EnableDetailedErrors = true
            });

            StartEventProcessor();
        }

        private void StartEventProcessor()
        {
            var tracker = new EventHubTrackerSource(s => { TrackerHub.Send(TrackerHub.Hub(), s); });
            tracker.StartAsync();
        }
    }
}