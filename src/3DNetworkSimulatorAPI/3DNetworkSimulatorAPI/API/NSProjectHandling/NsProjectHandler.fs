namespace _3DNetworkSimulatorAPI.API.NSProjectHandling

open FSharp.Json
open Microsoft.AspNetCore.Http
open Microsoft.EntityFrameworkCore.Design
open Microsoft.FSharp.Core
open MyDbContext
open Newtonsoft.Json
open Giraffe
open Microsoft.AspNetCore.Identity
open System.Linq
open _3DNetworkSimulatorAPI.Models
open _3DNetworkSimulatorAPI.Models.Models
open _3DNetworkSimulatorAPI.Util
open _3DNetworkSimulatorAPI.HttpHandler
open _3DNetworkSimulatorAPI.JsonUtil

module NSProjectHandler =
    type NSProjectHandler(dbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>, settings, logger, ownershipCheck) =
        let getContentString (ctx: HttpContext) =
            (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously

        let getUser (ctx: HttpContext) =
            task {
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                return! userManager.GetUserAsync ctx.User
            }

        member this.listProjects next ctx =
            task {
                let! user = getUser ctx
                
                use dbContext = dbContextFactory.CreateDbContext [||]
                let nsProjects = dbContext.NSProject.Where(fun x -> x.OwnerId = user.Id)
                let projectsJson = JsonConvert.SerializeObject nsProjects
                
                return! text projectsJson next ctx
            }
            
        member this.updateProject next ctx =
            task {
                let! user = getUser ctx

                use dbContext = dbContextFactory.CreateDbContext [||]

                let saveToDb x = 
                    let oldProject = dbContext.NSProject.Where(fun o -> o.Id = x.Id).First ()
                    
                    if oldProject.OwnerId = user.Id then
                        oldProject.JsonAnnotation <- x.JsonAnnotation
                        oldProject.GnsId <- x.GnsID
                        oldProject.Name <- x.Name
                        dbContext.Update oldProject |> ignore
                        dbContext.SaveChanges() |> ignore
                        Ok x
                        
                    else
                        Error (403, "Project does not belong to you")

                let res = getContentString ctx |> jsonDeserialize<NSProjectRaw> |> Result.bind saveToDb
                
                match res with
                | Ok _ ->
                    return! text "success" next ctx
                    
                | Error e ->
                    ctx.SetStatusCode (fst e)
                    return! text (snd e) next ctx
            }
            
        member this.createNewProject next ctx =
            task {
                let sendRequest request =
                    try
                        let resp = (sendGnsRequest request settings logger)
                        Ok resp
                    with _ ->
                        Error (503, "GNS3 is offline")
                
                let! user = getUser ctx
                
                (* Maybe add additional validation, like so that user has no more than N projects *)
                
                use dbContext = dbContextFactory.CreateDbContext [||]
                let userProjectCount = dbContext.NSProject.Where(fun x -> x.OwnerId = user.Id).Count ()
                let allProjectCount = dbContext.NSProject.Count ()
                
                let innerProjectName = $"{user.Id}_{userProjectCount}"
                let projectName = $"Project {userProjectCount + 1}"
                
                let creationRequest = POST ([ "v2"; "projects" ], $"{{\"name\": \"{innerProjectName}\"}}")
                let requestResult = sendRequest creationRequest
               
                let nsProjOfGnsProj pr = Ok {
                    Id = allProjectCount + 1
                    Name = projectName
                    GnsId = pr.project_id
                    OwnerId = user.Id
                    JsonAnnotation = "[]"
                }
                    
                let project = requestResult
                              |> Result.bind jsonDeserialize<GnsProject>
                              |> Result.bind nsProjOfGnsProj

                match project with
                | Ok project ->
                    dbContext.Add project |> ignore
                    dbContext.SaveChanges() |> ignore
                    return! text "success" next ctx
                
                | Error e ->
                    ctx.SetStatusCode (fst e)
                    return! text (snd e) next ctx
            }
