namespace _3DNetworkSimulatorAPI.GnsHandling

open FSharp.Json

module GnsSettings =
    type gnsSettings = {
        Protocol : string;
        BaseIP : string;
        Port : int;
        User : string;
        Password : string;
    }

    let getAddrBegin (currentGnsSettings : gnsSettings) = 
        let int2String x = sprintf "%i" x in
        currentGnsSettings.Protocol + "://" + currentGnsSettings.BaseIP + ":" + (int2String currentGnsSettings.Port)

    let getAuth (currentGnsSettings : gnsSettings) = 
        currentGnsSettings.User + ":" + currentGnsSettings.Password

    let fromJson jsonString = 
        Json.deserialize<gnsSettings> jsonString

    let toJson settings =
        Json.serialize settings