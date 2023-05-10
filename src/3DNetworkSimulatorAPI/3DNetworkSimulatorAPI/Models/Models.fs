namespace _3DNetworkSimulatorAPI.Models

open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

module Models =
    open Microsoft.AspNetCore.Identity

    [<CLIMutable>]
    type LoginModel =
        { [<Required>]
          Username: string
          [<Required>]
          Password: string }

    [<CLIMutable>]
    type TokenResult =
        { [<Required>]
          Token: string }

    [<CLIMutable>]
    type NSProject =
        { Id: int
          [<Required>]
          Name: string
          [<Required>]
          GnsId: string
          [<Required>]
          OwnerId: string
          [<Required>]
          JsonAnnotation: string }

    [<CLIMutable>]
    type NSProjectRaw =
        { Id: int
          [<Required>]
          Name: string
          [<Required>]
          GnsID: string
          [<Required>]
          JsonAnnotation: string }

    [<CLIMutable>]
    type RegisterModel =
        { Email: string
          Username: string
          Password: string }
