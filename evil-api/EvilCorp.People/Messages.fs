namespace EvilCorp.People

open System

module Messages = 

    type Calibration = {
        Id: string 
        X: int
        Y : int
        Timestamp : DateTime
    }

    type Info = {
        Id: string 
        Name: string
        PictureId : string
        Timestamp : DateTime
    }

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
        Name : string
        PictureId : string
        X : int
        Y : int
        Ssid : string[]
        Locations : Location[]
    }

