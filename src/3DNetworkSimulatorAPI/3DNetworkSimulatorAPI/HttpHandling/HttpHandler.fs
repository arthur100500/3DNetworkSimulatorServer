﻿namespace _3DNetworkSimulatorAPI

open FsHttp
open _3DNetworkSimulatorAPI.GnsHandling.GnsSettings
open Util

module HttpHandler =
    type GnsHttpRequest =
        | POST of string list * string
        | GET of string list
        | DELETE of string list

    let private buildUri (settings: gnsSettings) (parts: list<string>) =
        List.fold (fun p n -> p + "/" + n) (getAddrBegin settings) parts

    let private makePostRequest (endpoint: string) (jsonData: string) =
        printfn "Post to: %s" endpoint

        http {
            POST endpoint
            CacheControl "no-cache"
            body
            json jsonData
        }

    let private makeGetRequest (endpoint: string) =
        printfn "Get to: %s" endpoint

        http {
            GET endpoint
            CacheControl "no-cache"
            body
        }

    let private makeDeleteRequest (endpoint: string) =
        printfn "Delete to: %s" endpoint

        http {
            DELETE endpoint
            CacheControl "no-cache"
            body
        }

    let sendGnsRequest request currentGnsSettings =
        let createRequest =
            function
            | GET (uriList) -> buildUri currentGnsSettings uriList |> makeGetRequest
            | POST (uriList, data) -> (buildUri currentGnsSettings uriList |> makePostRequest) data
            | DELETE (uriList) -> buildUri currentGnsSettings uriList |> makeDeleteRequest in

        let sendRequest request =
            task {
                let! response = request |> Request.sendAsync
                return response
            } in

        let getResponseText request =
            let response = sendRequest request in
            let content = response.Result.content in
            let textContent = (content.ReadAsStream()) |> streamToStr |> Async.RunSynchronously in
            textContent in

        createRequest request |> getResponseText
