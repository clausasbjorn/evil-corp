﻿    namespace EvilCorp.People.Interface

    open System

    type Presence = {
        Identifier : string ;
        Channel : int ;
        Signal : int ;
        Hotspot : string ;
        Ssid : string ;
        Timestamp : DateTime
    }