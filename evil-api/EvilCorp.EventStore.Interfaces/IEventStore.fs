namespace EvilCorp.EventStore.Interfaces

open Microsoft.ServiceFabric.Services.Remoting
open System.Threading.Tasks

type IEventStore =
    inherit IService
    
    abstract member Store : string -> Task<unit>