namespace _3DNetworkSimulatorAPI

open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open _3DNetworkSimulatorAPI.GnsHandling.GnsHandler
open _3DNetworkSimulatorAPI.GnsHandling
open _3DNetworkSimulatorAPI.Logger;
open Microsoft.AspNetCore.Http
open WebSocketApp.Middleware
open Microsoft.AspNetCore.Cors
open System
open System.Threading.Tasks

module Program =
    let exitCode = 0

    let logger = new ConsoleLogger()

    let reqs =
        let configs = "Config/" in
        let settings = File.ReadAllText(configs + "gnsconfig.json") |> GnsSettings.fromJson in
        new GnsHandler(settings, logger)

    let checkOwnership = fun y z -> y z

    let displayNotFound next (ctx: HttpContext) =
        (text (HttpContextExtensions.GetRequestUrl ctx)) next ctx

    let apiEndpoints =
        choose
            [ subRoute
                  "/v2"
                  choose[GET
                         >=> choose
                             [ route "/" >=> checkOwnership >=> (warbler (fun _ -> text "This is an API"))
                               route "/projects" >=> checkOwnership >=> (reqs.projectsGet ())
                               routef "/projects/%s/nodes" (fun x -> checkOwnership >=> reqs.nodesGet x)
                               routef "/projects/%s/links" (fun x -> checkOwnership >=> reqs.linksGet x)
                               routef "/projects/%s/links/%s" (fun x -> checkOwnership >=> reqs.linksIDGet x) ]

                         POST
                         >=> choose
                             [ route "/projects" >=> checkOwnership >=> (reqs.projectsPost ())
                               routef "/projects/%s/open" (fun x -> checkOwnership >=> reqs.projectsOpenPost x)
                               routef "/projects/%s/nodes" (fun x -> checkOwnership >=> reqs.nodesPost x)
                               routef "/projects/%s/nodes/%s" (fun x -> checkOwnership >=> reqs.nodesIdPost x)
                               routef "/projects/%s/nodes/%s/start" (fun x -> checkOwnership >=> reqs.nodesStartPost x)
                               routef "/projects/%s/nodes/%s/stop" (fun x -> checkOwnership >=> reqs.nodesStopPost x)
                               routef "/projects/%s/links" (fun x -> checkOwnership >=> reqs.linksPost x) ]

                         DELETE
                         >=> choose
                             [ routef "/projects/%s/nodes/%s" (fun x -> checkOwnership >=> reqs.nodesIdDelete x)
                               routef "/projects/%s/links/%s" (fun x -> checkOwnership >=> reqs.linksIDDelete x) ]

                         routef "/projects/%s/nodes/%s/console/ws" (fun _ -> checkOwnership >=> reqs.webConsole logger)] ]


    let configureApp (app: IApplicationBuilder) =
        app.UseRouting().UseWebSockets().UseMiddleware<WebSocketMiddleware>() |> ignore

        app.UseCors(
            Action<_>(fun (builder: Infrastructure.CorsPolicyBuilder) ->
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore)
        )
        |> ignore

        app.UseGiraffe apiEndpoints

    let configureServices (services: IServiceCollection) =
        services
            .AddCors(fun options ->
                options.AddPolicy(
                    "Policy",
                    Action<Infrastructure.CorsPolicyBuilder>(fun (builder: Infrastructure.CorsPolicyBuilder) ->
                        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore)
                ))
            .AddGiraffe()
        |> ignore

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
