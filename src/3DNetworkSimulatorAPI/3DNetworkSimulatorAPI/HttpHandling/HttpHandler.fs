namespace _3DNetworkSimulatorAPI

open FsHttp
open GnsSettings;
open System.Net.Http
open Util

module HttpHandler =
    type GnsHttpRequest = 
      | POST of string list * string
      | GET of string list
      | DELETE of string list

    let globalGnsSettings : gnsSettings = {
        Protocol = "http";
        BaseIP = "127.0.0.1";
        Port = 3080;
        User = "admin";
        Password = "666";
    }

    let buildUri (parts : list<string>) = 
        List.fold (fun p n -> p + "/" + n) (getAddrBegin globalGnsSettings) parts 

    let makePostRequest (endpoint : string) (jsonData : string) =
        printfn "Post to: %s" endpoint
        http {
            POST endpoint
            CacheControl "no-cache"
            body
            json jsonData
        }

    let makeGetRequest (endpoint : string) =
        printfn "Get to: %s" endpoint
        http {
            GET endpoint
            CacheControl "no-cache"
            body
        }

    let makeDeleteRequest (endpoint : string) =
        printfn "Delete to: %s" endpoint
        http {
            DELETE endpoint
            CacheControl "no-cache"
            body
        }

    let sendRequest request =
        task {
            let! response = request |> Request.sendAsync
            return response
        }

    let getResponseText request =
        let response = sendRequest request in
        let content = response.Result.content in
        let textContent = (content.ReadAsStream ()) |> streamToStr |> Async.RunSynchronously in
        textContent

    let sendGnsRequest request = 
        let createRequest = function
            | GET (uriList) -> buildUri uriList |> makeGetRequest
            | POST (uriList, data) -> (buildUri uriList |> makePostRequest) data
            | DELETE (uriList) -> buildUri uriList |> makeDeleteRequest 
        in
        createRequest request |> getResponseText