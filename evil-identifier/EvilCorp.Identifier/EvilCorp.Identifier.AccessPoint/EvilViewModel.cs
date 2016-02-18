using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.AspNet.SignalR.Client;
using PropertyChanged;

namespace EvilCorp.Identifier.AccessPoint
{
    [ImplementPropertyChanged]
    internal class EvilViewModel
    {
        private readonly CoreDispatcher _dispatcher;
        private IHubProxy proxy;

        public EvilViewModel()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            var connection = new HubConnection("http://localhost:5781/signalr");
            //var connection = new HubConnection("http://time-share-robot-server.azurewebsites.net/signalr");
            proxy = connection.CreateHubProxy("EvilHub");

            RegisterEventHandlers(proxy);
            connection.Start();
        }

        private void RegisterEventHandlers(IHubProxy proxy)
        {
            // Register event for setting the QR-code for client URL
            proxy.On<Uri>("SetQrCode", SetQrCode);

            // Register events for controlling the robot
            proxy.On<string>("TakePicture", TakePicture);
        }

        private async void SetQrCode(Uri url)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                ControlQrc = new BitmapImage(url)
            );
        }


        private async void TakePicture(string appConnectionId)
        {
            var imageId = Guid.NewGuid();
            //TODO: take and upload image
            await proxy.Invoke("PictureTaken", appConnectionId, imageId.ToString());
        }

        public BitmapImage ControlQrc { get; set; }
    }
}