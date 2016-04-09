namespace EvilCorp.Ingest

open Owin;
open System
open System.Globalization
open System.Fabric
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open System.Net.Http.Formatting;
open System.Net.Http.Headers;
open System.Web.Http;
open Microsoft.Owin.Hosting;
open Microsoft.ServiceFabric.Services.Communication.Runtime;
open Microsoft.ServiceFabric.Services.Runtime;

type OwinAppBuilder() =
    member private this.ConfigureFormatters(formatters : MediaTypeFormatterCollection) =
        formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"))
        
    member this.Configuration(appBuilder : IAppBuilder) =
        let config = new HttpConfiguration()

        config.MapHttpAttributeRoutes()
        config.EnableCors()

        this.ConfigureFormatters(config.Formatters)
        
        appBuilder.UseWebApi(config) |> ignore

type OwinCommunicationListener(appRoot : string, startup : OwinAppBuilder, parameters : ServiceInitializationParameters) =
    let mutable serverHandle : Option<IDisposable> = None
    
    member private this.Stop() = 
        match serverHandle with
        | None -> ()
        | Some handle ->
            handle.Dispose()
            ()

    interface ICommunicationListener with

        member this.Abort() = 
            this.Stop()

        member this.CloseAsync _ = 
            this.Stop(); 
            Task.FromResult(true) :> Task

        member this.OpenAsync cancellationToken = 
            let endpoint = parameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint")
            let port = endpoint.Port;
            let root = 
                match String.IsNullOrWhiteSpace(appRoot) with
                | true -> "" 
                | false -> appRoot.TrimEnd('/') + "/"
            
            let listeningAddress = sprintf "http://+:%d/%s" port root
            let publishAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN)

            serverHandle <- Some (WebApp.Start(listeningAddress, fun appBuilder -> startup.Configuration(appBuilder)))

            Task.FromResult(publishAddress)

type Ingest() = 
    inherit StatelessService()
    override this.CreateServiceInstanceListeners () =
        [| ServiceInstanceListener(fun initParams -> OwinCommunicationListener("", OwinAppBuilder(), initParams) :> ICommunicationListener) |] 
        :> IEnumerable<ServiceInstanceListener>
