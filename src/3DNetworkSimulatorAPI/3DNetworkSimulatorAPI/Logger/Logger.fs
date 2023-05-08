namespace _3DNetworkSimulatorAPI.Logger

type ILogger =
    abstract Log: message: string -> unit
    abstract LogF: message: Printf.TextWriterFormat<'T> -> 'T

type ConsoleLogger() = 
    interface ILogger with
        member this.Log s = printfn "%s" s
        member this.LogF s = printfn s