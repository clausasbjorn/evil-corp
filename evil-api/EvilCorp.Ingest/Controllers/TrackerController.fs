namespace EvilCorp.Ingest.Controllers

open System
open System.Web.Http
open System.Threading.Tasks
open System.Net.Http
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors
open FSharp.Data
open EvilCorp.People.Interface
open Microsoft.ServiceFabric.Actors

module Tracker =

    type EventParser = JsonProvider<""" {"SSID": "Swebus 0044", "Antenna": 1, "timestamp": "2016-02-18T18:53:30.845789", "Rate": 2, "node_name": "b8:27:eb:fc:80:ea", "source": "6c:40:08:b2:7e:de", "b14": 0, "ChannelNumber": 1, "Flags": 0, "dBm_AntSignal": -71, "Channel": 2412, "target": "ff:ff:ff:ff:ff:ff"} """>
    
    let private parse json = 
        json |> EventParser.Parse

    let push json =
        let presence =
            json 
            |> parse
            |> (fun o -> { Identifier = o.Source ; Channel = o.ChannelNumber ; Signal = o.DBmAntSignal ; Timestamp = o.Timestamp })

        let person =
            presence.Identifier
            |> ActorId
            |> PersonFactory.createPerson 

        presence |> person.Seen |> ignore

        ()

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

            try
                Tracker.push json
            with
            | :? Exception -> ()

            return true
        } |> Async.StartAsTask

