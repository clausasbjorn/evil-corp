open System
open System.Threading
open System.Threading.Tasks
open System.Fabric
open Microsoft.ServiceFabric.Actors
open EvilCorp.People

[<EntryPoint>]
let main argv = 

    try
        use fabricRuntime = FabricRuntime.Create()

        fabricRuntime.RegisterActor<Person>()

        Thread.Sleep(Timeout.Infinite)
    with
    | :? FabricException as e -> ()
    | :? Exception as e -> ()

    0
