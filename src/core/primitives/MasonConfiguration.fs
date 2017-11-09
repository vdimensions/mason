namespace Mason

open System
open System.IO

module MasonConfiguration =

    [<Literal>]
    let DefaultConfigFileName: string = "mason.properties";
    [<Literal>]
    let SolutionFilePattern: string = "*.sln";

    [<Literal>]
    let ContextPropertyProjectFileName = "mason.context.project-file";
    [<Literal>]
    let ContextPropertySolutionDirName = "mason.context.solution-dir";
    [<Literal>]
    let ContextPropertyLocationName = "mason.context.location";

    let strNonEmpty(str: string) =
        match null2opt str with
        | Some s -> if (s.Length = 0) then None else Some s
        | None -> None

    let Get(location: string, projectName: string) =
        let mutable configs: IMasonProperties array = [||]
        let buildConfigFileName = DefaultConfigFileName
        
        let contextConfig = ContextProperties()
        contextConfig.[ContextPropertyProjectFileName] <- projectName

        match strNonEmpty location with
        | Some loc ->

            contextConfig.[ContextPropertyLocationName] <- loc

            let mutable locationDir = DirectoryInfo(loc)
            let projectSpecificConfigs = locationDir.GetFiles(String.Format("{0}.{1}", projectName, buildConfigFileName), SearchOption.TopDirectoryOnly)
            if (projectSpecificConfigs.Length = 1) then configs <- Array.append configs [|JavaProperties(projectSpecificConfigs.[0])|]

            let mutable shouldTraverse: bool = true
            while (shouldTraverse) do
                let defaultConfigs = locationDir.GetFiles(buildConfigFileName, SearchOption.TopDirectoryOnly)
                if (defaultConfigs.Length = 1) then configs <- Array.append configs [|JavaProperties(defaultConfigs.[0])|]

                let solutionFiles = locationDir.GetFiles(SolutionFilePattern, SearchOption.TopDirectoryOnly)
                if (solutionFiles.Length > 0) then 
                    contextConfig.[ContextPropertySolutionDirName] <- locationDir.FullName
                    shouldTraverse <- false
                else
                    locationDir <- locationDir.Parent
                    if (locationDir = null) then shouldTraverse <- false
        | None -> ()

        configs <- Array.append configs [|
                contextConfig;
                EnvironmentProperties(EnvironmentVariableTarget.Process); 
                EnvironmentProperties(EnvironmentVariableTarget.User); 
                EnvironmentProperties(EnvironmentVariableTarget.Machine)
            |]
        ExpandableProperties(PropertiesChain(configs))




