open Mason.Sdk
open Mason.Sdk.Options
open Mono.Options

type MonoOptionMap(args: string array) = 
    interface IOptionMap with
        member __.TryResolve(name:string): bool*'a =
            let mutable m = (false, Unchecked.defaultof<'a>)
            OptionSet().Add<'a>(name, (fun x -> m <- (true, x))).Parse(args) |> ignore
            m

type MonoOptionParser() =
    interface IOptionParser with member ___.Parse(args) = MonoOptionMap(args) :> IOptionMap

[<EntryPoint>]
let main argv =
    MasonRunner.Run(MonoOptionParser(), argv)
    0
