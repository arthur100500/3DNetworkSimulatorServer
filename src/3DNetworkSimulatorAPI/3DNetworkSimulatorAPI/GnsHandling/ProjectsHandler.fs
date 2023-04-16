namespace _3DNetworkSimulatorAPI

open Giraffe
open Microsoft.AspNetCore.Http
open Util
open HttpHandler

module ProjectsHandler =
    let getContentString (ctx : HttpContext) =
         (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously 

    let projectsGet () : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = GET ["v2"; "projects"] in
                return! (text (sendGnsRequest request)) next ctx
            }

    let projectsPost () : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"], getContentString ctx) in
                return! (text (sendGnsRequest request)) next ctx
            }


    let projectsOpenPost project_id : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let request = POST (["v2"; "projects"; project_id; "open" ], getContentString ctx) in
                return! (text (sendGnsRequest request)) next ctx
            }





