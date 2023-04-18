namespace _3DNetworkSimulatorAPI.GnsHandling

open FSharp.Json

module GnsSettings =
    type gnsSettings =
        { Protocol: string
          WsProtocol: string
          BaseIP: string
          Port: int
          User: string
          Password: string }

    let getAddrBegin (currentGnsSettings: gnsSettings) =
        let int2String x = $"%i{x}" in
        let stringPort = int2String currentGnsSettings.Port in
        $"{currentGnsSettings.Protocol}://{currentGnsSettings.BaseIP}:{stringPort}"

    let getWsAddrBegin (currentGnsSettings: gnsSettings) =
        let int2String x = $"%i{x}" in
        let stringPort = int2String currentGnsSettings.Port in
        $"{currentGnsSettings.WsProtocol}://{currentGnsSettings.BaseIP}:{stringPort}"

    let getAuth (currentGnsSettings: gnsSettings) =
        currentGnsSettings.User + ":" + currentGnsSettings.Password

    let fromJson jsonString =
        Json.deserialize<gnsSettings> jsonString

    let toJson settings = Json.serialize settings
