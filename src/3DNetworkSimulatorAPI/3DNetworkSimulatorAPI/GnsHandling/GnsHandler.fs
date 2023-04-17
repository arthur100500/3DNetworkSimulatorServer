namespace _3DNetworkSimulatorAPI.GnsHandling

open Microsoft.AspNetCore.Http
open Giraffe
open _3DNetworkSimulatorAPI.Util
open _3DNetworkSimulatorAPI.HttpHandler

module GnsHandler = 
    type GnsHandler(settings) =
        let createRequestTask request next ctx =
            task {
                return! (text (sendGnsRequest request settings)) next ctx
            }

        let getContentString (ctx : HttpContext) =
            (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously 

        member this.projectsGet () : HttpHandler =
            createRequestTask (GET ["v2"; "projects"])

        member this.projectsPost () : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"], getContentString ctx)) next ctx

        member this.projectsOpenPost project_id : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "open" ], getContentString ctx)) next ctx

        member this.nodesGet project_id : HttpHandler =
            createRequestTask (GET ["v2"; "projects"; project_id; "nodes"])

        member this.nodesPost project_id : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "nodes"], (getContentString ctx))) next ctx

        member this.nodesIdPost (project_id, node_id) : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "nodes"; node_id], (getContentString ctx))) next ctx

        member this.nodesIdDelete (project_id, node_id) : HttpHandler =
            createRequestTask (DELETE ["v2"; "projects"; project_id; "nodes"; node_id])

        member this.nodesStopPost (project_id, node_id) : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "nodes"; node_id; "stop"], (getContentString ctx))) next ctx

        member this.nodesStartPost (project_id, node_id) : HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "nodes"; node_id; "start"], (getContentString ctx))) next ctx

        member this.linksGet project_id : HttpHandler =
            createRequestTask (GET ["v2"; "projects"; project_id; "links"])

        member this.linksPost project_id: HttpHandler =
            fun (next : HttpFunc) (ctx : HttpContext) ->
                createRequestTask (POST (["v2"; "projects"; project_id; "links"], (getContentString ctx))) next ctx

        member this.linksIDGet (project_id, link_id): HttpHandler =
            createRequestTask (GET ["v2"; "projects"; project_id; "links"; link_id])

        member this.linksIDDelete (project_id, link_id): HttpHandler =
            createRequestTask (DELETE ["v2"; "projects"; project_id; "links"; link_id])