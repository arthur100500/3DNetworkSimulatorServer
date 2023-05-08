namespace _3DNetworkSimulatorAPI

open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure;
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

module Program =
    let exitCode = 0

    let logger = new ConsoleLogger()

    let reqs =
        let configs = "Config/" in
        let settings = File.ReadAllText(configs + "gnsconfig.json") |> GnsSettings.fromJson in
        new GnsHandler(settings, logger)

    let displayNotFound next (ctx: HttpContext) =
        (text (HttpContextExtensions.GetRequestUrl ctx)) next ctx

    let apiEndpoints =
        choose
            [ subRoute
                  "/v2"
                  choose[GET
                         >=> choose
                             [ route "/" >=> (warbler (fun _ -> text "This is an API"))
                               route "/projects" >=> (reqs.projectsGet ())
                               routef "/projects/%s/nodes" reqs.nodesGet
                               routef "/projects/%s/links" reqs.linksGet
                               routef "/projects/%s/links/%s" reqs.linksIDGet ]

                         POST
                         >=> choose
                             [ route "/projects" >=> (reqs.projectsPost ())
                               routef "/projects/%s/open" reqs.projectsOpenPost
                               routef "/projects/%s/nodes" reqs.nodesPost
                               routef "/projects/%s/nodes/%s" reqs.nodesIdPost
                               routef "/projects/%s/nodes/%s/start" reqs.nodesStartPost
                               routef "/projects/%s/nodes/%s/stop" reqs.nodesStopPost
                               routef "/projects/%s/links" reqs.linksPost ]

                         DELETE
                         >=> choose
                             [ routef "/projects/%s/nodes/%s" reqs.nodesIdDelete
                               routef "/projects/%s/links/%s" reqs.linksIDDelete ]

                         routef "/projects/%s/nodes/%s/console/ws" (fun _ -> reqs.webConsole logger)] ]


    let configureApp (app: IApplicationBuilder) =
        let allowAll = 
            fun (builder: Infrastructure.CorsPolicyBuilder) ->
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore

        let cors = Action<_>(allowAll)

        app.UseRouting()
            .UseWebSockets()
            .UseMiddleware<WebSocketMiddleware>()
            .UseAuthentication()
            .UseCors(cors)
            |> ignore

        app.UseGiraffe apiEndpoints


    let configureServices (services: IServiceCollection) =
        let allowAll = 
            fun (builder: Infrastructure.CorsPolicyBuilder) ->
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore

        let cors = 
            fun (options : Infrastructure.CorsOptions) ->
                options.AddPolicy ("Policy", Action<Infrastructure.CorsPolicyBuilder>(allowAll))  

        services
            .AddCors(cors)
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
