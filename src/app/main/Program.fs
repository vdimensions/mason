open Mason.Sdk
open Mason.Sdk.Options
open Mono.Options
open System

type MonoOptionMap(args: string array) = 
    interface IOptionMap with
        member __.Resolve(name:string): 'a =
            let mutable result = Unchecked.defaultof<'a>
            OptionSet().Add<'a>(name + "=", (fun x -> result <- x)).Parse(args) |> ignore
            result
        member __.ResolveFlag(name:string): bool =
            match args |> Array.tryFind (fun v -> StringComparer.OrdinalIgnoreCase.Equals(v, name)) with
            | Some _ -> true
            | None -> false

type MonoOptionParser() = interface IOptionParser with member ___.Parse(args) = MonoOptionMap(args) :> IOptionMap

[<EntryPoint>]
let main argv =
    MasonRunner.Run(MonoOptionParser(), argv)
    0
