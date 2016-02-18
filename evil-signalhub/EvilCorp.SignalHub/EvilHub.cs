using EvilCorp.SignalHub.Properties;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EvilCorp.SignalHub.GoogleApi;

namespace EvilCorp.SignalHub
{
    public class EvilHub : Hub
    {
        private readonly UrlShortener _urlShortener;

        public EvilHub()
        {
            _urlShortener = new UrlShortener();
        }


        // Commands from app
        public void TakePicture(string accessPointConnectionId)
        {
            Clients.Others.takePicture(Context.ConnectionId);
        }

        public void SaveIdentity(string mac, string name, string pictureId)
        {
            //TODO: Push to API
        }


        // Events from access point
        public void PictureTaken(string appConnectionId, string pictureId)
        {
            Clients.Client(appConnectionId).pictureTaken(Context.ConnectionId, pictureId);
        }
        
        // Set up on connection
        public override Task OnConnected()
        {
            Debug.WriteLine("Connected: " + Context.ConnectionId);
            var connectionId = Context.ConnectionId;
            var qrcUrl = _urlShortener.GetQrCodeForUrl(string.Format(Settings.Default.InputAppUrlFormat, connectionId));

            Clients.Client(connectionId).setQrCode(new Uri(qrcUrl, UriKind.Absolute));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Disconnected: " + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Debug.WriteLine("Reconnected: " + Context.ConnectionId);
            return (base.OnReconnected());
        }
    }
}