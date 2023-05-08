namespace _3DNetworkSimulatorAPI

open System.Text
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open _3DNetworkSimulatorAPI.Auth
open _3DNetworkSimulatorAPI.Routes
open WebSocketApp.Middleware
open Microsoft.AspNetCore.Cors
open System
open Microsoft.AspNetCore.Authentication.JwtBearer;
open Microsoft.IdentityModel.Tokens;
open Microsoft.EntityFrameworkCore.Sqlite
open Microsoft.EntityFrameworkCore;
open Microsoft.AspNetCore.Identity

module Program =
    let secret = Auth.secret

    let domain = Auth.domain

    let exitCode = 0

    let configureApp (app: IApplicationBuilder) =
        let allowAll = 
            fun (builder: Infrastructure.CorsPolicyBuilder) ->
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore

        let cors = Action<_>(allowAll)

        app.UseRouting()
            .UseWebSockets()
            .UseMiddleware<WebSocketMiddleware>()
            .UseAuthentication()
            .UseStaticFiles()
            .UseCors(cors)
            |> ignore

        app.UseGiraffe Routes.apiEndpoints


    let configureServices (services: IServiceCollection) =
        let allowAll = 
            fun (builder: Infrastructure.CorsPolicyBuilder) ->
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore

        let cors = 
            fun (options : Infrastructure.CorsOptions) ->
                options.AddPolicy ("Policy", Action<Infrastructure.CorsPolicyBuilder>(allowAll))  

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.TokenValidationParameters <- TokenValidationParameters(
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = domain,
                    ValidAudience = domain,
                    IssuerSigningKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)))
                ) |> ignore

        services.AddDbContext<MyDbContext.ApplicationDbContext>(fun options ->  
            options.UseSqlite("Data Source=identity.db") |> ignore
        ) |> ignore

        services.AddIdentity<IdentityUser, IdentityRole>(fun options -> 
                options.Password.RequireLowercase <- true
                options.Password.RequireUppercase <- true
                options.Password.RequireDigit <- true
                options.Lockout.MaxFailedAccessAttempts <- 10
                options.Lockout.DefaultLockoutTimeSpan <- TimeSpan.FromMinutes(5)
                options.User.RequireUniqueEmail <- true
            )
            .AddEntityFrameworkStores<MyDbContext.ApplicationDbContext>()
            .AddDefaultTokenProviders()
            |> ignore

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
