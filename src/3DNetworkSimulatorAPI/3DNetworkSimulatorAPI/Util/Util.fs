namespace _3DNetworkSimulatorAPI

open System.Text
open System.IO

module Util =
    let time() = System.DateTime.Now.ToString()

    let streamToStr (stream : Stream) = 
        async {
            let mutable result = ""
            use reader = new StreamReader(stream, Encoding.UTF8)
            do reader.ReadToEndAsync() |> Async.AwaitTask
            |> Async.RunSynchronously
            |> fun s -> result <- s
            return result
        }