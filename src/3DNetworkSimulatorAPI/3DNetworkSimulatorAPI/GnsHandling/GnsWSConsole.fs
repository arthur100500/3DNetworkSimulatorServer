namespace _3DNetworkSimulatorAPI

open System.Net.WebSockets
open System
open System.Threading

module GnsWSConsole = 
    type GnsWSConsole(ws : WebSocket) =
        let displayMessage (buffer : ArraySegment<byte>) =
            let response = System.Text.Encoding.ASCII.GetString(buffer)
            printfn "Recieved: %s" response

        let rec waitLoop () =
            let mutable buffer = new ArraySegment<byte>(Array.zeroCreate 2000);
            let result = ws.ReceiveAsync (buffer, CancellationToken.None)
                             |> Async.AwaitTask
                             |> Async.RunSynchronously
             
            match result.MessageType with
            | WebSocketMessageType.Text -> 
                displayMessage buffer;
                waitLoop ()
            | WebSocketMessageType.Close -> 
                printfn "Websocket closed"
                ()
            | _ ->
                printfn "Unsopported type"
                ()

        let startListening = 
            task { waitLoop () }

        member this.Start () =
            startListening |> Async.AwaitTask |> Async.RunSynchronously


