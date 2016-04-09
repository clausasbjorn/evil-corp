namespace EvilCorp.Ingest.Controllers

open System
open System.Web.Http
open System.Web.Http.Cors
open System.Threading.Tasks
open System.Net.Http
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors
open FSharp.Data
open EvilCorp.Phones.Interfaces
open EvilCorp.Stats.Interfaces

module Tracker =
    
    type EventParser = JsonProvider<""" {"SSID": "Swebus 0044", "Antenna": 1, "timestamp": "2016-02-18T18:53:30.845789", "Rate": 2, "node_name": "b8:27:eb:fc:80:ea", "source": "6c:40:08:b2:7e:de", "b14": 0, "ChannelNumber": 1, "Flags": 0, "dBm_AntSignal": -71, "Channel": 2412, "target": "ff:ff:ff:ff:ff:ff" } """>
    
    let private parse json = 
        json 
        |> EventParser.Parse
        |> (fun o -> new ProbeRequest(o.Source, o.Ssid))

    let push json =
        let probe =
            json 
            |> parse

        let phone =
            probe.Identifier
            |> ActorId
            |> PhoneFactory.CreatePhone

        phone.Update(probe.Ssid)

type TrackerController() =
    inherit ApiController()

    [<HttpGet>]
    [<Route("")>]
    member this.Index() =
        "Tracker API"

    [<HttpPost>]
    [<Route("track")>]
    member this.Track(request : HttpRequestMessage) = 
        async {
            let! json = request.Content.ReadAsStringAsync() |> Async.AwaitTask
            
            Tracker.push json |> ignore
            
            return true
        } |> Async.StartAsTask

    [<HttpGet>]
    [<Route("stats")>]
    [<EnableCors("*", "*", "GET")>]
    member this.Stats() = 
        async {
            return StatsFactory.CreateStats().WhoAreNearby();
        } |> Async.StartAsTask