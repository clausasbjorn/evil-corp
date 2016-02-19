namespace EvilCorp.People

open Microsoft.ServiceBus.Messaging

module EventPush =

    let private connectionString = "Endpoint=sb://relay-dev.servicebus.windows.net/;SharedAccessKeyName=SendRule;SharedAccessKey=jgHFGM/uEC+KxYfk004dxPWJC5INfhKV5dP+yesKY/Q="
    let private eventHubName = "evil-hub"
    let private hub = EventHubClient.CreateFromConnectionString(connectionString, eventHubName)
    
    let private toBytes (message : string) = 
        System.Text.Encoding.UTF8.GetBytes(message) 
        
    let send message =
        let bytes = message |> toBytes
        let event = new EventData(bytes)
        hub.Send(event)

