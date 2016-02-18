namespace EvilCorp.People.Interface

open System
open Microsoft.ServiceFabric.Actors

module PersonFactory =

    let personUrl = new Uri("fabric:/EvilCorp/PersonActorService");
    
    let createPerson actorId =
        ActorProxy.Create<IPerson>(actorId, personUrl)
        
         
