namespace _3DNetworkSimulatorAPI

open System.Net.WebSockets
open System
open System.Text
open WebSocketSharp
open System.Net
open System.Threading
open _3DNetworkSimulatorAPI.GnsHandling.GnsSettings


module GnsWSConsole =
    type GnsWSConsole(ws: WebSockets.WebSocket, url: string, settings: gnsSettings) =
        let transformUrl (url: string) (settings: gnsSettings) =
            let sliced = (url.Split "v2")

            match Array.length sliced with
            | 1 -> raise (new Exception("URL was not designed for GNS3, as \"v2\" substring was not present"))
            | n ->
                let furtherPart = String.Join ("v2", sliced[1..])
                let addrBegin = getWsAddrBegin settings
                $"{addrBegin}/v2{furtherPart}"


        let createGnsWS (url: string) = new WebSocketSharp.WebSocket(url)

        let rec mirrorLoop (gnsWs: WebSocketSharp.WebSocket) =
            let mutable buffer = new ArraySegment<byte>(Array.zeroCreate 2000)

            let result =
                ws.ReceiveAsync(buffer, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously

            let sendMessage =
                let response = Encoding.ASCII.GetString(buffer)
                gnsWs.Send response
                mirrorLoop gnsWs

            match result.MessageType with
            | WebSocketMessageType.Text -> sendMessage
            | WebSocketMessageType.Binary -> sendMessage
            | WebSocketMessageType.Close -> ()
            | _ -> printfn "Unsopported type"

        let messageResend (e: MessageEventArgs) =
            let sendBytes dataBytes =
                let dataBytesSegment = ArraySegment<byte> dataBytes

                ws.SendAsync(dataBytesSegment, WebSocketMessageType.Binary, true, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously

            match e.IsBinary, e.IsText, e.IsPing with
            | true, false, false ->
                let dataBytes = e.RawData
                sendBytes dataBytes
            | false, true, false ->
                let dataBytes = Encoding.ASCII.GetBytes e.Data
                sendBytes dataBytes
            | false, false, true ->
                let dataBytesSegment = ArraySegment<byte> e.RawData

                ws.SendAsync(dataBytesSegment, WebSocketMessageType.Close, true, CancellationToken.None)
                |> Async.AwaitTask
                |> Async.RunSynchronously
            | _ -> ()

        let startListening =
            let transformedUrl = transformUrl url settings

            task {
                printfn "Connecting %s and %s" url transformedUrl
                let gnsWs = createGnsWS transformedUrl
                gnsWs.OnMessage.Add messageResend
                gnsWs.Connect()
                mirrorLoop gnsWs
                printfn "Disconnecting %s and %s" url transformedUrl
            }

        member this.Start() =
            startListening |> Async.AwaitTask |> Async.RunSynchronously
