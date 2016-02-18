namespace EvilCorp.People.Interface

open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors

type IPerson =
    inherit IActor

    abstract member Seen : presence : Presence -> Task