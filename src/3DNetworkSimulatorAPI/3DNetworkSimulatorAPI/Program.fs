namespace _3DNetworkSimulatorAPI

open System.IO;
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open _3DNetworkSimulatorAPI.GnsHandling.GnsHandler
open _3DNetworkSimulatorAPI.GnsHandling;
open Microsoft.AspNetCore.Http
open WebSocketApp.Middleware

module Program =
    let exitCode = 0

    let reqs = 
        let configs = "Config/" in
        let settings = File.ReadAllText (configs + "gnsconfig.json") |> GnsSettings.fromJson in
        new GnsHandler(settings)

    let displayNotFound next (ctx : HttpContext) = 
        (text (HttpContextExtensions.GetRequestUrl ctx)) next ctx

    let apiEndpoints =
        choose [
            subRoute "/v2" choose[ 
                GET >=> choose [
                     (* General *)
                     route "/" >=> (warbler (fun _ -> text "This is an API"))
                     (* Projects *)
                     route "/projects" >=> (reqs.projectsGet ())
                     (* Nodes *)
                     routef "/projects/%s/nodes" reqs.nodesGet
                     (* Links *)
                     routef "/projects/%s/links" reqs.linksGet
                     routef "/projects/%s/links/%s" reqs.linksIDGet
                ]
                POST >=> choose [
                     (* Projects *)
                     route "/projects" >=> (reqs.projectsPost ())
                     routef "/projects/%s/open" reqs.projectsOpenPost
                     (* Nodes *)
                     routef "/projects/%s/nodes" reqs.nodesPost
                     routef "/projects/%s/nodes/%s" reqs.nodesIdPost
                     routef "/projects/%s/nodes/%s/start" reqs.nodesStartPost
                     routef "/projects/%s/nodes/%s/stop" reqs.nodesStopPost
                     (* Links *)
                     routef "/projects/%s/links" reqs.linksPost 
                ]
                DELETE >=> choose [
                     (* Nodes *)
                     routef "/projects/%s/nodes/%s" reqs.nodesIdDelete
                     (* Links *)
                     routef "/projects/%s/links/%s" reqs.linksIDDelete
                ] 
                routef "/projects/%s/nodes/%s/console/ws" reqs.webConsole
                (* NOT_FOUND "Not found" >=> displayNotFound *)
            ]
        ]
        

    let configureApp (app: IApplicationBuilder) =
        app.UseRouting()
            .UseWebSockets()
            .UseMiddleware<WebSocketMiddleware>()
        |> ignore
        app.UseGiraffe apiEndpoints

    let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore


    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)
        configureServices builder.Services

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        configureApp app
        app.Run()

        exitCode
