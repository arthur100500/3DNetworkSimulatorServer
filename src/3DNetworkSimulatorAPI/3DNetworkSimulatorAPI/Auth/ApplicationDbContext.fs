module MyDbContext

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design

type ApplicationDbContext(options : DbContextOptions<ApplicationDbContext>) = 
    inherit IdentityDbContext(options)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member __.CreateDbContext (args: string[]) =
            let optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlite("Data Source=identity.db") |> ignore
            new ApplicationDbContext(optionsBuilder.Options)
