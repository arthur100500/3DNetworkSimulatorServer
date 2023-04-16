namespace _3DNetworkSimulatorAPI

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.EndpointRouting
open ProjectsHandler
open LinksHandler
open NodesHandler

module Program =
    let exitCode = 0

    let apiEndpoints = [
        subRoute "/v2" [
            GET [
                route "/" (warbler (fun _ -> text "This is an API"))

                (* Projects *)
                route "/projects" (projectsGet ())
                
                (* Nodes *)
                routef "/projects/%s/nodes" nodesGet
               
                (* Links *)
                routef "/projects/%s/links" linksGet
                routef "/projects/%s/links/%s" linksIDGet
            ]
            POST [
                (* Projects *)
                route "/projects" (projectsPost ())
                routef "/projects/%s/open" projectsOpenPost

                (* Nodes *)
                routef "/projects/%s/nodes" nodesPost
                routef "/projects/%s/nodes/%s" nodesIdPost
                routef "/projects/%s/nodes/%s/start" nodesStartPost
                routef "/projects/%s/nodes/%s/stop" nodesStopPost

                (* Links *)
                routef "/projects/%s/links" linksPost
            ]
            DELETE [
                (* Nodes *)
                routef "/projects/%s/nodes/%s" nodesIdDelete

                (* Links *)
                routef "/projects/%s/links/%s" linksIDDelete
            ]
        ]
    ]

    let configureApp (app : IApplicationBuilder) =
        app.UseRouting()
           .UseEndpoints(fun e -> e.MapGiraffeEndpoints(apiEndpoints))
        |> ignore

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore


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