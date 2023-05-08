namespace _3DNetworkSimulatorAPI.Models
open System.ComponentModel.DataAnnotations

module Models =
    [<CLIMutable>]
    type LoginModel =
        {
            Id : int
            [<Required>] Username : string
            [<Required>] Password : string
        }

    [<CLIMutable>]
    type TokenResult =
        {
            [<Required>] Token : string
        }

    [<CLIMutable>]
    type GnsProject =
        {
            Id : int
            [<Required>] Name : string
            [<Required>] OwnderID : int
            [<Required>] JsonAnnotation : string
        }