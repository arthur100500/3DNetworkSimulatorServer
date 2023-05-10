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
    type GnsProject =
        { Id: int
          [<Required>]
          Name: string
          [<Required>]
          OwnderID: int
          [<Required>]
          JsonAnnotation: string }

    [<CLIMutable>]
    type RegisterModel =
        { Email: string
          Username: string
          Password: string }
