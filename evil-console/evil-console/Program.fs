module Program

open Microsoft.ServiceBus.Messaging
open System
open System.Threading
open System.Threading.Tasks
open EvilConsole

[<EntryPoint>]
let main argv = 

    let eventHubConnectionString = "Endpoint=sb://relay-dev.servicebus.windows.net/;SharedAccessKeyName=ReceiveRule;SharedAccessKey=FBJkNG0oPTYQW2eGnd4LQJSFtpneYZpFB4jT+2136bw="
    let eventHubName = "evil-hub"
    let storageAccountName = "evilstorage"
    let storageAccountKey = "UAX64PuM4JC3BPRMQ7bcDbfs3yXq6TlEceG6v1IYK3inAemzMqwP3OPwEF1XtDtk4xNDem2NWqgZVsQ6gzXK6Q=="
    let storageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey)

    let eventProcessorHostName = Guid.NewGuid().ToString()
    let eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString)
    Console.WriteLine("Registering EventProcessor...")
    eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>().Wait()

    Console.WriteLine("Receiving. Press enter key to stop worker.")
    Console.ReadLine()
    eventProcessorHost.UnregisterEventProcessorAsync().Wait();

    0 // return an integer exit code
