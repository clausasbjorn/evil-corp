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

type Status =
    | Present = 0uy
    | NotPresent = 1uy

[<DataContract>]
[<CLIMutable>]
type PersonState = {
    [<DataMember>] Identifier : string ; 
    [<DataMember>] LastSeen : DateTime ;
    [<DataMember>] Status : byte  }

module PersonBehavior =
    let received presence state =
        state

    let initPerson actorId =
        { Identifier = actorId
          LastSeen = DateTime.UtcNow
          Status = (byte Status.Present) }
            
type Person() = 
    inherit StatefulActor<PersonState>()

    override this.OnActivateAsync() = 
        this.State <- this.GetActorId().GetStringId() |> PersonBehavior.initPerson
        base.OnActivateAsync()

    interface IPerson with

        member this.Seen(presence) =
            this -!> (PersonBehavior.received presence)




