namespace Mason.Sdk
open Mason

[<AbstractClass>]
type AbstractSettings(properties: IMasonProperties) =
    member __.Location with get() = properties.[MasonConfiguration.ContextPropertyLocationName]
    member __.ProjectFile with get() = properties.[MasonConfiguration.ContextPropertyProjectFileName]
    member __.SolutionDir with get() = properties.[MasonConfiguration.ContextPropertySolutionDirName]


