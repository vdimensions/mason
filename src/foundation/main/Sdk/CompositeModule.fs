namespace Mason.Sdk

open System
open Mason
open Mason.Sdk.Options

[<AbstractClass>]
type CompositeModule(name: string, submodules: seq<IMasonModule>) as self =
    do
        match null2opt name with | None -> nullArg "name" | Some _ -> ()
        match null2opt submodules with | None -> nullArg "submodules" | Some _ -> ()
    new(name: string, [<ParamArray>] submodules: IMasonModule array) = CompositeModule(name, submodules :> seq<IMasonModule>)
    abstract member ResolveModules: options: IOptionMap * submodules: seq<IMasonModule> -> seq<IMasonModule>
    default __.ResolveModules(options: IOptionMap, submodules: seq<IMasonModule>) =
        submodules |> Seq.map(fun x -> if options.ResolveFlag(x.Name) then Some x else None) |> Seq.choose id


    member __.Run(properties: IMasonProperties, options: IOptionMap) =
        for sm in __.ResolveModules(options, submodules) do sm.Run(properties, options)

    interface IMasonModule with
        member __.Run(properties, options) = self.Run(properties, options)
        member __.Name with get() = name