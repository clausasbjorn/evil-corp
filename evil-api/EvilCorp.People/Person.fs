namespace EvilCorp.People

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.Serialization
open Microsoft.ServiceFabric
open Microsoft.ServiceFabric.Actors
open EvilCorp.Common.Actors
open EvilCorp.People.Interface
open EvilCorp.People.Messages
open Newtonsoft.Json

type Status =
    | Present = 0uy
    | NotPresent = 1uy

[<DataContract>]
[<CLIMutable>]
type Location = {
    [<DataMember>] Hotspot : string ;
    [<DataMember>] LastSeen : DateTime ;
    [<DataMember>] Channel : int ;
    [<DataMember>] FirstSeen : DateTime ;
    [<DataMember>] Signal : int }

[<DataContract>]
[<CLIMutable>]
type PersonState = {
    [<DataMember>] Count : int ;
    [<DataMember>] Identifier : string ; 
    [<DataMember>] LastSeen : DateTime ;
    [<DataMember>] Status : byte ;
    [<DataMember>] Name : string ;
    [<DataMember>] PictureId : string ;
    [<DataMember>] X : int ;
    [<DataMember>] Y : int ;
    [<DataMember>] Ssid : List<string> ;
    [<DataMember>] Locations : Dictionary<string, Location> }

module PersonBehavior =

    let hotspot (presence : Presence) state = 
        let locations = state.Locations 
        match locations.ContainsKey(presence.Hotspot) with
        | true -> state.Locations.[presence.Hotspot] <- { locations.[presence.Hotspot] with LastSeen = presence.Timestamp ; Channel = presence.Channel ; Signal = presence.Signal }
        | false -> state.Locations.Add(presence.Hotspot, { Hotspot = presence.Hotspot ; LastSeen = presence.Timestamp ; Channel = presence.Channel ; Signal = presence.Signal ; FirstSeen = DateTime.UtcNow })

        match state.Count with
        | 4 ->
            state.Locations.Keys
            |> Seq.map(fun key -> 
                let location = state.Locations.[key]
                { Hotspot = location.Hotspot ; LastSeen = location.LastSeen ; Channel = location.Channel ; SignalStrength = location.Signal }
            )
            |> (fun locations -> 
                { Id = state.Identifier ; Name = state.Name ; PictureId = state.PictureId ; X = state.X ; Y = state.Y ; Ssid = Seq.toArray(state.Ssid) ; Locations = Seq.toArray(locations) }
            )
            |> JsonConvert.SerializeObject
            |> EventPush.send 

            { state with Count = 0 }
        | _ ->
            { state with Count = state.Count + 1 }

    let addSsid (ssid : string) state = 
        match state.Ssid.Contains(ssid) with
        | true -> ()
        | false -> state.Ssid.Add(ssid)
        
    let lastSeen state =
        { Id = state.Identifier ; LastSeen = state.LastSeen }
        |> JsonConvert.SerializeObject
        |> EventPush.send 
        
    let received (presence : Presence) state =
        let update = { state with Status = (byte Status.Present) ; LastSeen = presence.Timestamp }
        update |> (addSsid presence.Ssid)
        update |> (hotspot presence)

    let updateInfo (info : EvilCorp.People.Interface.Info) (state : PersonState) =
        let update = { state with Name = info.Name ; PictureId = info.PictureId }
        
        { Id = update.Identifier ; Name = update.Name ; PictureId = update.PictureId ; Timestamp = DateTime.UtcNow }
        |> JsonConvert.SerializeObject
        |> EventPush.send 

        update

    let calibrate (calibration : EvilCorp.People.Interface.Calibration) (state : PersonState) =
        let update = { state with X = calibration.X ; Y = calibration.Y }
        
        { Id = update.Identifier ; X = update.X ; Y = update.Y ; Timestamp = DateTime.UtcNow }
        |> JsonConvert.SerializeObject
        |> EventPush.send 

        update

    let initPerson actorId =
        { Count = 0
          Identifier = actorId
          LastSeen = DateTime.UtcNow
          Status = (byte Status.Present)
          Name = ""
          PictureId = ""
          X = -1
          Y = -1
          Ssid = new List<string>()
          Locations = new Dictionary<string, Location>() }
            
type Person() = 
    inherit StatefulActor<PersonState>()

    override this.OnActivateAsync() = 
        this.State <- this.GetActorId().GetStringId() |> PersonBehavior.initPerson
        base.OnActivateAsync()

    interface IPerson with

        member this.Seen(presence) =
            this -!> (PersonBehavior.received presence)

        member this.SetInfo(info) =
            this -!> (PersonBehavior.updateInfo info)

        member this.Calibrate(calibration) =
            this -!> (PersonBehavior.calibrate calibration)




