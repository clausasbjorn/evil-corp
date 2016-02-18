namespace EvilCorp.Ingest.Controllers

open System
open System.Web.Http
open System.Threading.Tasks
open System.Net.Http
open System.Threading.Tasks
open EvilCorp.EventStore.Interfaces

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
            let! json = request.Content.ReadAsStringAsync() |> Async.AwaitTask
            let eventStore = EventStoreConnectionFactory.CreateEventStoreConnection()
            eventStore.Store(json) |> ignore
            return true
        } |> Async.StartAsTask

