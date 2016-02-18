namespace EvilCorp.Ingest.Controllers

open System
open System.Web.Http
open System.Threading.Tasks
open System.Net.Http
open System.Threading.Tasks
open Microsoft.ServiceBus.Messaging
open EvilCorp.People.Interface
open Microsoft.ServiceFabric.Actors

//open EvilCorp.EventStore.Interfaces

module EventHub =
    let connectionString = "Endpoint=sb://relay-dev.servicebus.windows.net/;SharedAccessKeyName=SendRule;SharedAccessKey=jgHFGM/uEC+KxYfk004dxPWJC5INfhKV5dP+yesKY/Q="
    let eventHubName = "evil-hub"
    let hub = EventHubClient.CreateFromConnectionString(connectionString, eventHubName)
    
    let toBytes (message : string) = 
        System.Text.Encoding.UTF8.GetBytes(message) 
        
    let send message =
        let bytes = message |> toBytes
        let event = new EventData(bytes)
        hub.Send(event)

    let call message =
        PersonFactory.createPerson (new ActorId(""))

type TrackerController() =
    inherit ApiController()

    [<HttpGet>]
    [<Route("")>]
    member this.Index() =
        "EvilCorp Tracker"

    [<HttpPost>]
    [<Route("track")>]
    member this.Track(request : HttpRequestMessage) = 
        async {
            return true
//            let! json = request.Content.ReadAsStringAsync() |> Async.AwaitTask
//
//            try
//                EventHub.send json
//            with
//            | :? Exception -> ()
//
//            return true
        } |> Async.StartAsTask

