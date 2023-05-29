namespace _3DNetworkSimulatorAPI.API.NSProjectHandling

open FSharp.Json
open Microsoft.AspNetCore.Http
open Microsoft.EntityFrameworkCore.Design
open MyDbContext
open Newtonsoft.Json
open Giraffe
open Microsoft.AspNetCore.Identity
open System.Linq
open _3DNetworkSimulatorAPI
open _3DNetworkSimulatorAPI.Models.Models
open _3DNetworkSimulatorAPI.Util
open _3DNetworkSimulatorAPI.HttpHandler

module NSProjectHandler =
    type NSProjectHandler(dbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>, settings, logger, ownershipCheck) =
        let getContentString (ctx: HttpContext) =
            (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously

        let getUser (ctx: HttpContext) =
            task {
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                return! userManager.GetUserAsync ctx.User
            }

        member this.listProjects() : HttpHandler =
            ownershipCheck
            >=> fun next ctx ->
                    task {
                        let! user = getUser ctx

                        use dbContext = dbContextFactory.CreateDbContext [||]
                        let nsProjects = dbContext.NSProject.Where(fun x -> x.OwnerId = user.Id)
                        let projectsJson = JsonConvert.SerializeObject(nsProjects)
                        return! text projectsJson next ctx
                    }

        member this.addProject() : HttpHandler =
            ownershipCheck
            >=> fun next ctx ->
                    task {
                        let! user = getUser ctx

                        use dbContext = dbContextFactory.CreateDbContext [||]

                        match getContentString ctx |> JsonUtil.jsonDeserialize<NSProjectRaw> with
                        | Some x ->
                            // TODO: Add user check
                            let oldProject = dbContext.NSProject.Where(fun o -> o.Id = x.Id).First ()

                            oldProject.JsonAnnotation <- x.JsonAnnotation
                            oldProject.GnsId <- x.GnsID
                            oldProject.Name <- x.Name

                            dbContext.Update oldProject |> ignore
                            dbContext.SaveChanges() |> ignore
                            return! text "success" next ctx
                        | None ->
                            ctx.SetStatusCode 400
                            return! text "Incorrect json" next ctx
                    }

        member this.addEmpty() : HttpHandler =
            ownershipCheck
            >=> fun next ctx ->
                    task {
                        let! user = getUser ctx
                        use dbContext = dbContextFactory.CreateDbContext [||]

                        let pId = dbContext.NSProject.Count () + 1

                        let nsProject: NSProject =
                            { Id = pId
                              Name = $"New project {pId}"
                              GnsId = "uninited"
                              OwnerId = user.Id
                              JsonAnnotation = "[]" }

                        dbContext.Add nsProject |> ignore
                        dbContext.SaveChanges() |> ignore
                        return! text "success" next ctx
                    }
