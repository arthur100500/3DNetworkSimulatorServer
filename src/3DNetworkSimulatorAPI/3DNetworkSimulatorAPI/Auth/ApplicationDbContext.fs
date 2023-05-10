module MyDbContext

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design
open Microsoft.AspNetCore.Identity
open _3DNetworkSimulatorAPI.Models.Models

type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

    [<DefaultValue>]
    val mutable NSProject: DbSet<NSProject>

    member this._NSProject
        with public get () = this.NSProject
        and public set value = this.NSProject <- value

    override _.OnModelCreating(modelBuilder: ModelBuilder) =
        modelBuilder
            .Entity<IdentityUser>()
            .Ignore("PhoneNumber")
            .Ignore("PhoneNumberConfirmed")
        |> ignore

        base.OnModelCreating(modelBuilder)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member _.CreateDbContext(args: string[]) =
            let optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlite("Data Source=identity.db") |> ignore
            new ApplicationDbContext(optionsBuilder.Options)
