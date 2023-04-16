namespace _3DNetworkSimulatorAPI

open Giraffe
open Microsoft.AspNetCore.Http
open Util
open HttpHandler

module LinksHandler =
    let getContentString (ctx : HttpContext) =
         (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously 

    let linksGet project_id : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = GET ["v2"; "projects"; project_id; "links"] in
                return! (text (sendGnsRequest request)) next ctx
            }

    let linksPost project_id: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "links"], (getContentString ctx)) in
                return! (text (sendGnsRequest request)) next ctx
            }

    let linksIDGet (project_id, link_id): HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = GET ["v2"; "projects"; project_id; "links"; link_id] in
                return! (text (sendGnsRequest request)) next ctx
            }

    let linksIDDelete (project_id, link_id): HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = DELETE ["v2"; "projects"; project_id; "links"; link_id] in
                return! (text (sendGnsRequest request)) next ctx
            }