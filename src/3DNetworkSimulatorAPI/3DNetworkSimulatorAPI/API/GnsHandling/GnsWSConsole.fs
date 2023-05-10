namespace _3DNetworkSimulatorAPI

open System.Net.WebSockets
open System
open System.Text
open WebSocketSharp
open System.Net
open System.Threading
open _3DNetworkSimulatorAPI.GnsHandling.GnsSettings
open _3DNetworkSimulatorAPI.Logger


module GnsWSConsole =
    type GnsWSConsole(ws: WebSockets.WebSocket, url: string, settings: gnsSettings, logger: ILogger) =
        let transformUrl (url: string) (settings: gnsSettings) =
            let sliced = (url.Split "v2")

            match Array.length sliced with
            | 1 -> raise (new Exception("URL was not designed for GNS3, as \"v2\" substring was not present"))
            | n ->
                let furtherPart = String.Join("v2", sliced[1..])
                let addrBegin = getWsAddrBegin settings
                $"{addrBegin}/v2{furtherPart}"


        let createGnsWS (url: string) = new WebSocketSharp.WebSocket(url)

        let rec mirrorLoop (gnsWs: WebSocketSharp.WebSocket) =
            let mutable buffer = new ArraySegment<byte>(Array.zeroCreate 2000)

            let result =
                ws.ReceiveAsync(buffer, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously

            let sendStringMessage =
                let response = Encoding.ASCII.GetString(buffer)
                gnsWs.Send response
                mirrorLoop gnsWs

            let sendByteMessage =
                let log = Encoding.ASCII.GetString(buffer)
                let response = buffer.ToArray()
                gnsWs.Send response
                mirrorLoop gnsWs

            match result.MessageType with
            | WebSocketMessageType.Text -> sendStringMessage
            | WebSocketMessageType.Binary -> sendByteMessage
            | _ -> ()

        let messageResend (e: MessageEventArgs) =
            let sendBytes (dataBytes: array<byte>) messageType =
                let log = Encoding.ASCII.GetString dataBytes
                let dataBytesSegment = ArraySegment<byte> dataBytes

                ws.SendAsync(dataBytesSegment, messageType, true, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously

            match e.IsBinary, e.IsText, e.IsPing with
            | true, false, false ->
                let dataBytes = e.RawData
                sendBytes dataBytes WebSocketMessageType.Binary
            | false, true, false ->
                let dataBytes = Encoding.ASCII.GetBytes e.Data
                sendBytes dataBytes WebSocketMessageType.Text
            | false, false, true ->
                let dataBytesSegment = ArraySegment<byte> e.RawData

                ws.SendAsync(dataBytesSegment, WebSocketMessageType.Close, true, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously
            | _ -> ()

        let startListening =
            let transformedUrl = transformUrl url settings

            task {
                logger.LogF "Connecting %s and %s" url transformedUrl
                let gnsWs = createGnsWS transformedUrl
                gnsWs.OnMessage.Add messageResend
                gnsWs.Connect()
                mirrorLoop gnsWs
                logger.LogF "Disconnecting %s and %s" url transformedUrl
            }

        member this.Start() =
            try
                startListening |> Async.AwaitTask |> Async.RunSynchronously
            with ex ->
                logger.LogF "Exception occured. Maybe this was an error"
