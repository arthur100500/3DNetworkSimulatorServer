namespace _3DNetworkSimulatorAPI.Auth

open System.Text
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Microsoft.EntityFrameworkCore.Design
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Authentication.JwtBearer
open Giraffe
open System
open MyDbContext
open _3DNetworkSimulatorAPI.Models
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Identity
open _3DNetworkSimulatorAPI.Models.Models
open _3DNetworkSimulatorAPI.Util
open System.Linq

module Auth =
    let secret = "12312312321312kflgsdflgnmsdflmg_REMOVE_LATER"

    let domain = "127.0.0.1:5000"

    let authorize: HttpHandler =
        requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)
        
    let getUser (ctx: HttpContext) =
        task {
            let userManager = ctx.GetService<UserManager<IdentityUser>>()
            return! userManager.GetUserAsync ctx.User
        }

    let generateToken email =
        let claims =
            [| Claim(JwtRegisteredClaimNames.Sub, email)
               Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

        let expires = Nullable(DateTime.UtcNow.AddHours(20.0))
        let notBefore = Nullable(DateTime.UtcNow)
        let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))

        let signingCredentials =
            SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

        let token =
            JwtSecurityToken(
                issuer = domain,
                audience = domain,
                claims = claims,
                expires = expires,
                notBefore = notBefore,
                signingCredentials = signingCredentials
            )

        let tokenResult: Models.TokenResult =
            { Token = JwtSecurityTokenHandler().WriteToken(token) }

        tokenResult

    let postTokenHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                //let! model = ctx.BindJsonAsync<Models.LoginModel>()
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                let! user = userManager.GetUserAsync ctx.User

                match user with
                | null ->
                    ctx.SetStatusCode 401
                    return! next ctx
                | some ->
                    let tokenResult = generateToken some.UserName
                    return! json tokenResult next ctx
            }

    let getContentString (ctx: HttpContext) =
        (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously

    let registerHandler: HttpHandler =
        fun next ctx ->
            task {
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                let content = getContentString ctx
                let data = FSharp.Json.Json.deserialize<RegisterModel> content
                let user = IdentityUser(Email = data.Email, UserName = data.Username)
                let! result = userManager.CreateAsync(user, data.Password)

                if result.Succeeded then
                    ctx.SetStatusCode 200
                    return! text ("success") next ctx
                else
                    let errors = result.Errors |> Seq.map (fun e -> e.Description) |> List.ofSeq
                    ctx.SetStatusCode 400
                    return! text (String.Join(" ", errors)) next ctx
            }

    let loginHandler: HttpHandler =
        fun next ctx ->
            task {
                let content = getContentString ctx
                let data = FSharp.Json.Json.deserialize<LoginModel> content
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                let! result = signInManager.PasswordSignInAsync(data.Username, data.Password, true, false)

                if result.Succeeded then
                    ctx.SetStatusCode 200
                    return! text "success" next ctx
                else
                    ctx.SetStatusCode 400
                    return! text "bad login or password" next ctx
            }

    let logoutHandler: HttpHandler =
        fun next ctx ->
            task {
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                do! signInManager.SignOutAsync()

                return! text "success" next ctx
            }
            
    let getJwt (dbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>) : HttpHandler =
        fun next ctx ->
            task {
                let content = getContentString ctx
                let data = FSharp.Json.Json.deserialize<LoginModel> content
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                let! result = signInManager.PasswordSignInAsync(data.Username, data.Password, true, false)
                
                let findUser () =
                    use dbContext = dbContextFactory.CreateDbContext [||]
                    dbContext.Users.Where(fun x -> x.UserName = data.Username).FirstOrDefault()
                    
                if result.Succeeded then
                    ctx.SetStatusCode 200
                    let user = findUser ()
                    return! text (generateToken user.Id).Token next ctx
                else
                    ctx.SetStatusCode 400
                    return! text "bad login or password" next ctx
            }
            
    let testJwt : HttpHandler =
        fun next ctx ->
            task {
                let userId = ctx.User.FindFirst ClaimTypes.NameIdentifier
                printfn $"Here is: %s{userId.Value}"
                return! text userId.Value next ctx
            }
