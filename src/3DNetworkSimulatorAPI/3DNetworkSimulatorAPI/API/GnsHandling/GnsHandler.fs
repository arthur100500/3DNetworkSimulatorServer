namespace _3DNetworkSimulatorAPI.GnsHandling

open Microsoft.AspNetCore.Http
open Giraffe
open _3DNetworkSimulatorAPI.Util
open _3DNetworkSimulatorAPI.HttpHandler
open _3DNetworkSimulatorAPI.GnsWSConsole
open System.Threading.Tasks
open FsHttp
open System.Net.WebSockets
open System.Collections.Generic

module GnsHandler =
    type GnsHandler(settings, logger, ownershipCheck) =
        let createRequestTask request =
            ownershipCheck >=> fun next (ctx: HttpContext) ->
            task {
                try
                    let resp = (text (sendGnsRequest request settings logger))
                    return! resp next ctx
                with
                    | _ -> 
                        ctx.SetStatusCode 503
                        return! (text "GNS3 is off on the server") next ctx
            }

        let getContentString (ctx: HttpContext) =
            (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously

        member this.projectsGet() : HttpHandler =
            createRequestTask (GET [ "v2"; "projects" ])

        member this.projectsPost() : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask (POST([ "v2"; "projects" ], getContentString ctx)) next ctx

        member this.projectsOpenPost project_id : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask (POST([ "v2"; "projects"; project_id; "open" ], getContentString ctx)) next ctx

        member this.nodesGet project_id : HttpHandler =
            createRequestTask (GET [ "v2"; "projects"; project_id; "nodes" ])

        member this.nodesPost project_id : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask (POST([ "v2"; "projects"; project_id; "nodes" ], (getContentString ctx))) next ctx

        member this.nodesIdPost(project_id, node_id) : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask
                    (POST([ "v2"; "projects"; project_id; "nodes"; node_id ], (getContentString ctx)))
                    next
                    ctx

        member this.nodesIdDelete(project_id, node_id) : HttpHandler =
            createRequestTask (DELETE [ "v2"; "projects"; project_id; "nodes"; node_id ])

        member this.nodesStopPost(project_id, node_id) : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask
                    (POST([ "v2"; "projects"; project_id; "nodes"; node_id; "stop" ], (getContentString ctx)))
                    next
                    ctx

        member this.nodesStartPost(project_id, node_id) : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask
                    (POST([ "v2"; "projects"; project_id; "nodes"; node_id; "start" ], (getContentString ctx)))
                    next
                    ctx

        member this.linksGet project_id : HttpHandler =
            createRequestTask (GET [ "v2"; "projects"; project_id; "links" ])

        member this.linksPost project_id : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                createRequestTask (POST([ "v2"; "projects"; project_id; "links" ], (getContentString ctx))) next ctx

        member this.linksIDGet(project_id, link_id) : HttpHandler =
            createRequestTask (GET [ "v2"; "projects"; project_id; "links"; link_id ])

        member this.linksIDDelete(project_id, link_id) : HttpHandler =
            createRequestTask (DELETE [ "v2"; "projects"; project_id; "links"; link_id ])

        member this.webConsole(logger) : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) ->
                let url = (HttpContextExtensions.GetRequestUrl ctx)

                let acceptWebsocket (wsTask: Task<WebSocket>) =
                    task {
                        let ws = wsTask.Result in
                        let gnsWsConsole = new GnsWSConsole(ws, url, settings, logger) in
                        gnsWsConsole.Start() |> ignore
                    }
                    |> Async.AwaitTask
                    |> Async.RunSynchronously

                match ctx.WebSockets.IsWebSocketRequest with
                | false -> (setStatusCode 404) next ctx
                | true ->
                    printfn "WebSocket request: %s" url
                    acceptWebsocket (ctx.WebSockets.AcceptWebSocketAsync()) |> ignore
                    next ctx
