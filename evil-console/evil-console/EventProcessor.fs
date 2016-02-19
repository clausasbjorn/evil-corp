namespace EvilConsole

open Microsoft.ServiceBus.Messaging
open System
open System.Text
open System.Diagnostics
open System.Collections.Generic
open System.Threading.Tasks

type EventProcessor() =

    let mutable stopwatch : Stopwatch = new Stopwatch()

    interface IEventProcessor with
    
        member this.CloseAsync(context : PartitionContext, reason : CloseReason) = 
            async {
                match reason with
                | CloseReason.Shutdown -> 
                    context.CheckpointAsync() 
                    |> Async.AwaitTask 
                    |> ignore
                | _ -> ()
            } |> Async.StartAsTask :> Task
       

        member this.OpenAsync(context : PartitionContext) =
            stopwatch <- new Stopwatch()
            stopwatch.Start()
            Task.FromResult() :> Task

        member this.ProcessEventsAsync(context : PartitionContext, messages : IEnumerable<EventData>) = 
            async {
                messages
                |> Seq.iter(fun data -> 
                    let json = Encoding.UTF8.GetString(data.GetBytes())
                    Console.Write(".")
                    ()
                    //Console.WriteLine(String.Format("Message received: {0}", json))
                )

                match (stopwatch.Elapsed > TimeSpan.FromMinutes(1.0)) with
                | true -> 
                    context.CheckpointAsync() 
                    |> Async.AwaitTask
                    |> ignore

                    stopwatch.Restart()
                | false -> ()
            } |> Async.StartAsTask :> Task



