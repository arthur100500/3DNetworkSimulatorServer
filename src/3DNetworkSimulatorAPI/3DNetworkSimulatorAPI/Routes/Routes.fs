namespace _3DNetworkSimulatorAPI.Routes

open Giraffe
open _3DNetworkSimulatorAPI.API.NSProjectHandling.NSProjectHandler
open _3DNetworkSimulatorAPI.GnsHandling.GnsHandler
open _3DNetworkSimulatorAPI.GnsHandling
open _3DNetworkSimulatorAPI.Logger
open _3DNetworkSimulatorAPI.Auth
open System.IO
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Http
open System.Security.Claims

module Routes =
    let logger = ConsoleLogger()

    let configs = "Config/"

    let dbContextGen = MyDbContext.ApplicationDbContextFactory()

    let checkOwnership: HttpHandler =
        let printEmail next (ctx: HttpContext) =
            let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
            printfn "Email is here: %s" email.Value
            next ctx

        let authorize =
            requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

        // authorize >=> printEmail
        fun n c -> n c

    let reqs =
        let settings = File.ReadAllText(configs + "gnsconfig.json") |> GnsSettings.fromJson in
        GnsHandler(settings, logger, checkOwnership)
        
    let nsReqs = 
        let settings = File.ReadAllText(configs + "gnsconfig.json") |> GnsSettings.fromJson in
        NSProjectHandler(dbContextGen, settings, logger) 

    let nsProjectsRoutes =
        [ route "/projects" >=> (nsReqs.listProjects)
          route "/update" >=> (nsReqs.updateProject) 
          route "/new" >=> (nsReqs.createNewProject) ]

    let apiPostRoutes =
        [ route "/projects" >=> (reqs.projectsPost ())
          routef "/projects/%s/open" reqs.projectsOpenPost
          routef "/projects/%s/nodes" reqs.nodesPost
          routef "/projects/%s/nodes/%s" reqs.nodesIdPost
          routef "/projects/%s/nodes/%s/start" reqs.nodesStartPost
          routef "/projects/%s/nodes/%s/stop" reqs.nodesStopPost
          routef "/projects/%s/links" reqs.linksPost ]

    let apiGetRoutes =
        [ route "/" >=> (warbler (fun _ -> text "This is an API"))
          route "/projects" >=> (reqs.projectsGet ())
          routef "/projects/%s/nodes" reqs.nodesGet
          routef "/projects/%s/links" reqs.linksGet
          routef "/projects/%s/links/%s" reqs.linksIDGet ]

    let apiDeleteRoutes =
        [ routef "/projects/%s/nodes/%s" reqs.nodesIdDelete
          routef "/projects/%s/links/%s" reqs.linksIDDelete ]

    let apiAllRoutes =
        [ GET >=> choose apiGetRoutes
          POST >=> choose apiPostRoutes
          DELETE >=> choose apiDeleteRoutes
          routef "/projects/%s/nodes/%s/console/ws" (fun _ -> reqs.webConsole logger) ]

    let apiEndpoints: HttpHandler =
        choose
            [ subRoute "/v2" (choose apiAllRoutes)
              subRoute "/ns" (choose nsProjectsRoutes)
              route "/token" >=> Auth.postTokenHandler
              route "/register" >=> Auth.registerHandler
              route "/login" >=> Auth.loginHandler
              route "/logout" >=> Auth.logoutHandler ]
