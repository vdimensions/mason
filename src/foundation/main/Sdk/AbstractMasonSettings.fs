namespace Mason.Sdk

open System
open System.IO

open Mason

[<AbstractClass>]
type AbstractMasonSettings(properties: IMasonProperties) =
    [<Literal>]
    let RequiredPropertyMissingMessageFormat: string = "Required property '{0}' is not defined.";
    /// Gets a value representing the filesystem location of the current mason.properties config file.
    member __.Location with get() = __.GetRequiredProperty(MasonContext.Properties.LocationKey)
    /// Gets a value representing the filesystem location of the current project file.
    member __.ProjectFile with get() = properties.[MasonContext.Properties.ProjectFileKey]
    /// Gets a value representing the filesystem location of the current projects' solution.
    /// <remarks>Can be <c>null</c>.</remarks>
    member __.SolutionDir with get() = properties.[MasonContext.Properties.SolutionDirKey]
    /// Gets a value representing the name of the current mason config file.
    member __.ConfigFileName with get() = properties.[MasonContext.Properties.FileNameKey]
    member __.ConfigFile with get() = new FileInfo(Path.Combine(__.Location, __.ConfigFileName))
    member __.GetRequiredProperty key:string =
        match null2opt key with
        | Some k ->
            match null2opt properties.[k] with
            | Some v -> v
            | None -> invalidOp(String.Format(RequiredPropertyMissingMessageFormat, k))
        | None -> nullArg "key"


