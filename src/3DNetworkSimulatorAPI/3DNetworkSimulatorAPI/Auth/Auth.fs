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

        let tokenResult = {
            Token = JwtSecurityTokenHandler().WriteToken(token)
        }

        tokenResult

    let handleGetSecured =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
            
            text ("User " + email.Value + " is authorized to access this resource.") next ctx

    let handlePostToken =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! model = ctx.BindJsonAsync<LoginViewModel>()

                // authenticate user
            
                let tokenResult = generateToken model.Email

                return! json tokenResult next ctx
            }