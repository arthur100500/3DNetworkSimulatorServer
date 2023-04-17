namespace _3DNetworkSimulatorAPI

open FsHttp
open GnsSettings;
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

    let private buildUri (parts : list<string>) = 
        List.fold (fun p n -> p + "/" + n) (getAddrBegin globalGnsSettings) parts 

    let private makePostRequest (endpoint : string) (jsonData : string) =
        printfn "Post to: %s" endpoint
        http {
            POST endpoint
            CacheControl "no-cache"
            body
            json jsonData
        }

    let private makeGetRequest (endpoint : string) =
        printfn "Get to: %s" endpoint
        http {
            GET endpoint
            CacheControl "no-cache"
            body
        }

    let private makeDeleteRequest (endpoint : string) =
        printfn "Delete to: %s" endpoint
        http {
            DELETE endpoint
            CacheControl "no-cache"
            body
        }

    let sendGnsRequest request = 
        let createRequest = function
            | GET (uriList) -> buildUri uriList |> makeGetRequest
            | POST (uriList, data) -> (buildUri uriList |> makePostRequest) data
            | DELETE (uriList) -> buildUri uriList |> makeDeleteRequest 
        in
        let sendRequest request =
            task {
                let! response = request |> Request.sendAsync
                return response
            }
        in
        let getResponseText request =
            let response = sendRequest request in
            let content = response.Result.content in
            let textContent = (content.ReadAsStream ()) |> streamToStr |> Async.RunSynchronously in
            textContent
        in
        createRequest request |> getResponseText