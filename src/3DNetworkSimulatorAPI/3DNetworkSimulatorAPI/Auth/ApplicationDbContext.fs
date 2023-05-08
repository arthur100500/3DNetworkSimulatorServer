module MyDbContext

open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design

type ApplicationDbContext(options : DbContextOptions<ApplicationDbContext>) = 
    inherit IdentityDbContext(options)

    override __.OnModelCreating (modelBuilder : ModelBuilder) =
        base.OnModelCreating(modelBuilder)
        modelBuilder.Entity<IdentityRole>().HasData(
            [|
                IdentityRole(Name = "admin", NormalizedName = "ADMIN")
                IdentityRole(Name = "user", NormalizedName = "USER")
            |]) |> ignore

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member __.CreateDbContext (args: string[]) =
            let optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlite("Data Source=identity.db") |> ignore
            new ApplicationDbContext(optionsBuilder.Options)