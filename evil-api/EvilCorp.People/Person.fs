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
    [<DataMember>] Locations : Dictionary<string, Location> }

module PersonBehavior =

    let hotspot (presence : Presence) state = 
        let locations = state.Locations 
        match locations.ContainsKey(presence.Hotspot) with
        | true -> state.Locations.[presence.Hotspot] <- { locations.[presence.Hotspot] with LastSeen = presence.Timestamp ; Channel = presence.Channel ; Signal = presence.Signal }
        | false -> state.Locations.Add(presence.Hotspot, { Hotspot = presence.Hotspot ; LastSeen = presence.Timestamp ; Channel = presence.Channel ; Signal = presence.Signal ; FirstSeen = DateTime.UtcNow })

        match state.Count with
        | 20 ->
            state.Locations.Keys
            |> Seq.map(fun key -> 
                let location = state.Locations.[key]
                { Hotspot = location.Hotspot ; LastSeen = location.LastSeen ; Channel = location.Channel ; SignalStrength = location.Signal }
            )
            |> (fun locations -> 
                { Id = state.Identifier ; Locations = Seq.toArray(locations) }
            )
            |> JsonConvert.SerializeObject
            |> EventPush.send 

            { state with Count = 0 }
        | _ ->
            { state with Count = state.Count + 1 }

    let lastSeen state =
        { Id = state.Identifier ; LastSeen = state.LastSeen }
        |> JsonConvert.SerializeObject
        |> EventPush.send 
        
    let received (presence : Presence) state =
        let update = { state with Status = (byte Status.Present) ; LastSeen = presence.Timestamp }
        update |> (hotspot presence)

    let initPerson actorId =
        { Count = 0
          Identifier = actorId
          LastSeen = DateTime.UtcNow
          Status = (byte Status.Present)
          Locations = new Dictionary<string, Location>() }
            
type Person() = 
    inherit StatefulActor<PersonState>()

    override this.OnActivateAsync() = 
        this.State <- this.GetActorId().GetStringId() |> PersonBehavior.initPerson
        base.OnActivateAsync()

    interface IPerson with

        member this.Seen(presence) =
            this -!> (PersonBehavior.received presence)




