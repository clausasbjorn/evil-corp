namespace EvilCorp.People

open System

module Messages = 

    type LastSeen = {
        Id : string 
        LastSeen : DateTime }

    type Location = {
        Hotspot : string
        LastSeen : DateTime
        Channel : int
        SignalStrength : int }
    
    type Locations = {
        Id : string
        Locations : Location[]
    }

