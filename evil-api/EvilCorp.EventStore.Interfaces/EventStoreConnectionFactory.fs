namespace EvilCorp.EventStore.Interfaces

open System
open Microsoft.ServiceFabric.Services.Remoting.Client

module EventStoreConnectionFactory =

    let private serviceUrl = new Uri("fabric:/EvilCorp/EventStore")

    let CreateEventStoreConnection () =
        ServiceProxy.Create<IEventStore>(0L, serviceUrl);