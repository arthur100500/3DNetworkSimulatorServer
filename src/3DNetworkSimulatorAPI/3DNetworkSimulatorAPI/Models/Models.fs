namespace _3DNetworkSimulatorAPI.Models

open System.ComponentModel.DataAnnotations

module Models =
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
          mutable Name: string
          [<Required>]
          mutable GnsId: string
          [<Required>]
          OwnerId: string
          [<Required>]
          mutable JsonAnnotation: string }

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
