namespace _3DNetworkSimulatorAPI.Auth

open System.Text
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Authentication.JwtBearer
open Giraffe
open System
open _3DNetworkSimulatorAPI.Models
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Identity
open _3DNetworkSimulatorAPI.Models.Models
open _3DNetworkSimulatorAPI.Util
open FSharp.Json


module Auth =
    let secret = "12312312321312kflgsdflgnmsdflmg_REMOVE_LATER"

    let domain = "127.0.0.1:5000"
    
    let authorize : HttpHandler =
        requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

    let generateToken email =
        let claims = [|
            Claim(JwtRegisteredClaimNames.Sub, email);
            Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

        let expires = Nullable(DateTime.UtcNow.AddHours(1.0))
        let notBefore = Nullable(DateTime.UtcNow)
        let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        let signingCredentials = SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

        let token =
            JwtSecurityToken(
                issuer = domain,
                audience = domain,
                claims = claims,
                expires = expires,
                notBefore = notBefore,
                signingCredentials = signingCredentials)

        let tokenResult : Models.TokenResult = {
            Token = JwtSecurityTokenHandler().WriteToken(token)
        }

        tokenResult

    let postTokenHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                //let! model = ctx.BindJsonAsync<Models.LoginModel>()
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                let! user = userManager.GetUserAsync ctx.User
            
                let tokenResult = generateToken user.UserName

                return! json tokenResult next ctx
            }

    let getContentString (ctx: HttpContext) =
        (ctx.Request.Body |> streamToStr) |> Async.RunSynchronously

    let registerHandler : HttpHandler =
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
                    return! text (String.Join (" ", errors)) next ctx
            }

    let loginHandler : HttpHandler =
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

    let logoutHandler : HttpHandler =
        fun next ctx -> 
            task {
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                do! signInManager.SignOutAsync()

                return! text "success" next ctx
            }