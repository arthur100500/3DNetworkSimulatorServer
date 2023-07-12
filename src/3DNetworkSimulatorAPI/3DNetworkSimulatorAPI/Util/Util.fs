namespace _3DNetworkSimulatorAPI

open System.Text
open System.IO
open FSharp.Json

module Util =
    let time () = System.DateTime.Now.ToString()

    let streamToStr (stream: Stream) =
        async {
            let mutable result = ""
            use reader = new StreamReader(stream, Encoding.UTF8)

            do
                reader.ReadToEndAsync()
                |> Async.AwaitTask
                |> Async.RunSynchronously
                |> fun s -> result <- s

            return result
        }

module JsonUtil =
    let jsonDeserialize<'T> json =
        try
            Ok (Json.deserialize<'T> json)
        with _ ->
            Error (500, "Serialization Error")

    let jsonSerialize = Json.serialize
