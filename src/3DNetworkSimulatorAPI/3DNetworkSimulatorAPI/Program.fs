namespace _3DNetworkSimulatorAPI

open System.IO;
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.EndpointRouting
open _3DNetworkSimulatorAPI.GnsHandling.GnsHandler
open _3DNetworkSimulatorAPI.GnsHandling;

module Program =
    let exitCode = 0

    let reqs = 
        let configs = "Config/" in
        let settings = File.ReadAllText (configs + "gnsconfig.json") |> GnsSettings.fromJson in
        new GnsHandler(settings)

    let apiEndpoints =
        [ subRoute
              "/v2"
              [ GET
                    [
                      (* General *)
                      route "/" (warbler (fun _ -> text "This is an API"))

                      (* Projects *)
                      route "/projects" (reqs.projectsGet ())

                      (* Nodes *)
                      routef "/projects/%s/nodes" reqs.nodesGet

                      (* Links *)
                      routef "/projects/%s/links" reqs.linksGet
                      routef "/projects/%s/links/%s" reqs.linksIDGet ]
                POST
                    [
                      (* Projects *)
                      route "/projects" (reqs.projectsPost ())
                      routef "/projects/%s/open" reqs.projectsOpenPost

                      (* Nodes *)
                      routef "/projects/%s/nodes" reqs.nodesPost
                      routef "/projects/%s/nodes/%s" reqs.nodesIdPost
                      routef "/projects/%s/nodes/%s/start" reqs.nodesStartPost
                      routef "/projects/%s/nodes/%s/stop" reqs.nodesStopPost

                      (* Links *)
                      routef "/projects/%s/links" reqs.linksPost ]
                DELETE
                    [
                      (* Nodes *)
                      routef "/projects/%s/nodes/%s" reqs.nodesIdDelete

                      (* Links *)
                      routef "/projects/%s/links/%s" reqs.linksIDDelete ] ] ]

    let configureApp (app: IApplicationBuilder) =
        app.UseRouting().UseEndpoints(fun e -> e.MapGiraffeEndpoints(apiEndpoints))
        |> ignore

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
