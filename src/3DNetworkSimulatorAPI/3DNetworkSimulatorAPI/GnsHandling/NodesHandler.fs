namespace _3DNetworkSimulatorAPI

open Giraffe
open Microsoft.AspNetCore.Http
open Util
open HttpHandler

module NodesHandler =
    let getContentString (ctx : HttpContext) =
         (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously 

    let nodesGet project_id : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = GET ["v2"; "projects"; project_id; "nodes"] in
                return! (text (sendGnsRequest request)) next ctx
            }

    let nodesPost project_id : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "nodes"], (getContentString ctx)) in
                return! (text (sendGnsRequest request)) next ctx
            }

    let nodesIdPost (project_id, node_id) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "nodes"; node_id], (getContentString ctx)) in
                return! (text (sendGnsRequest request)) next ctx
            }

    let nodesIdDelete (project_id, node_id) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = DELETE ["v2"; "projects"; project_id; "nodes"; node_id] in
                return! (text (sendGnsRequest request)) next ctx
            }

    let nodesStopPost (project_id, node_id) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "nodes"; node_id; "stop"], (getContentString ctx)) in
                return! (text (sendGnsRequest request)) next ctx
            }

    let nodesStartPost (project_id, node_id) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "nodes"; node_id; "start"], (getContentString ctx)) in
                return! (text (sendGnsRequest request)) next ctx
            }