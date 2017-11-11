namespace Mason.Sdk

open System

open Mason

[<AbstractClass>]
type AbstractMasonSettings(properties: IMasonProperties) =
    [<Literal>]
    let RequiredPropertyMissingMessageFormat: string = "Required property '{0}' is not defined.";
    member __.Location with get() = __.GetRequiredProperty(MasonConfiguration.ContextPropertyLocationName)
    member __.ProjectFile with get() = properties.[MasonConfiguration.ContextPropertyProjectFileName]
    member __.SolutionDir with get() = properties.[MasonConfiguration.ContextPropertySolutionDirName]
    member __.ConfigFile with get() = properties.[MasonConfiguration.ContextPropertyFileName]
    member __.GetRequiredProperty key:string =
        match null2opt key with
        | Some k ->
            match null2opt properties.[k] with
            | Some v -> v
            | None -> invalidOp(String.Format(RequiredPropertyMissingMessageFormat, k))
        | None -> nullArg "key"


