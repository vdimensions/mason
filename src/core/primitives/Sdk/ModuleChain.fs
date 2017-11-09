namespace Mason.Sdk

open System

open Mason

type ModuleChain(name: string, selectiveExecution: bool, submodules: seq<IMasonModule>) as self =
    do
        if (obj.ReferenceEquals(name, null)) then raise (ArgumentNullException "name")
        if (obj.ReferenceEquals(submodules, null)) then raise (ArgumentNullException "submodules")
    new (name: string, selectiveExecution: bool, [<ParamArray>] modules: IMasonModule array) = ModuleChain(name, selectiveExecution, modules :> seq<IMasonModule>)

    member __.Run(properties: IMasonProperties, [<ParamArray>] args: string array) =
        match selectiveExecution with
        | true ->
            match List.ofSeq args with
            | start::remainder ->
                let r = Array.ofList remainder
                let candidateMultipleSubmodules = start.Split([|'&'|], StringSplitOptions.RemoveEmptyEntries)
                for candidateModule in candidateMultipleSubmodules do
                    let trimmed = candidateModule.Trim()
                    for m in submodules do
                        if (StringComparer.OrdinalIgnoreCase.Equals(m.Name, trimmed)) then
                            m.Run(properties, r)
        | false ->
            for m in submodules do
                m.Run(properties, args)

    member __.Name with get() = name

    interface IMasonModule with
        member __.Run(properties, args) = self.Run(properties, args)
        member __.Name with get() = name
