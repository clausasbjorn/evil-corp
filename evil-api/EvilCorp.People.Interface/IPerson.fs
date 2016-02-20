namespace EvilCorp.People.Interface

open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors

type IPerson =
    inherit IActor

    abstract member Seen : presence : Presence -> Task
    abstract member SetInfo : info : Info -> Task
    abstract member Calibrate : calibration : Calibration -> Task