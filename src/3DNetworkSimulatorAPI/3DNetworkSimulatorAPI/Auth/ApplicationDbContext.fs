module MyDbContext

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design
open Microsoft.AspNetCore.Identity

type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

    override __.OnModelCreating(modelBuilder: ModelBuilder) =
        modelBuilder
            .Entity<IdentityUser>()
            .Ignore("PhoneNumber")
            .Ignore("PhoneNumberConfirmed")
        |> ignore

        base.OnModelCreating(modelBuilder)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member __.CreateDbContext(args: string[]) =
            let optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlite("Data Source=identity.db") |> ignore
            new ApplicationDbContext(optionsBuilder.Options)
