namespace EvilCorp.EventStore

open Microsoft.ServiceFabric.Data
open Microsoft.ServiceFabric.Data.Collections
open Microsoft.ServiceFabric.Services.Communication.Runtime
open Microsoft.ServiceFabric.Services.Runtime
open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric.Services.Remoting.Runtime
open Microsoft.ServiceFabric.Services.Remoting

open EvilCorp.EventStore.Interfaces

type EventStore() = 
    inherit StatefulService()
    
    override __.CreateServiceReplicaListeners () =
        [| ServiceReplicaListener(fun initParams -> new ServiceRemotingListener<EventStore>(initParams, __) :> ICommunicationListener) |] 
        :> IEnumerable<ServiceReplicaListener>

    interface IEventStore with
        
        member __.Store (json : string) =
            async {
                use tx = __.StateManager.CreateTransaction()
                let! incoming = __.StateManager.GetOrAddAsync<IReliableQueue<string>>(tx, "incoming") |> Async.AwaitTask
                incoming.EnqueueAsync(tx, json) |> Async.AwaitTask |> ignore
                return ()
            } |> Async.StartAsTask
