namespace Mason

open System
open System.IO

module MasonConfiguration =
    [<Literal>]
    let DefaultConfigFileName: string = "mason.properties"
    [<Literal>]
    let SolutionFilePattern: string = "*.sln"


    let strNonEmpty(str: string) =
        match null2opt str with
        | Some s -> if (s.Length = 0) then None else Some s
        | None -> None

    let Get(location: string, projectName: string) =
        let mutable configs: IMasonProperties array = [||]
        let buildConfigFileName = DefaultConfigFileName
        
        let contextConfig = ContextProperties()
        contextConfig.[MasonContext.Properties.ProjectFileKey] <- projectName

        match strNonEmpty location with
        | Some loc ->

            contextConfig.[MasonContext.Properties.LocationKey] <- loc

            let mutable locationDir = DirectoryInfo(loc)
            let projectConfigFileName = String.Format("{0}.{1}", projectName, buildConfigFileName);
            let projectSpecificConfigs = locationDir.GetFiles(projectConfigFileName, SearchOption.TopDirectoryOnly)
            if (projectSpecificConfigs.Length = 1) then 
                configs <- Array.append configs [|JavaProperties(projectSpecificConfigs.[0])|]
                contextConfig.[MasonContext.Properties.FileNameKey] <- projectConfigFileName              

            let mutable shouldTraverse: bool = true
            while (shouldTraverse) do
                let defaultConfigs = locationDir.GetFiles(buildConfigFileName, SearchOption.TopDirectoryOnly)
                if (defaultConfigs.Length = 1) then 
                    configs <- Array.append configs [|JavaProperties(defaultConfigs.[0])|]
                    match null2opt contextConfig.[MasonContext.Properties.FileNameKey] with
                    | Some _ -> ()
                    | _ -> contextConfig.[MasonContext.Properties.FileNameKey] <- buildConfigFileName

                let solutionFiles = locationDir.GetFiles(SolutionFilePattern, SearchOption.TopDirectoryOnly)
                if (solutionFiles.Length > 0) then 
                    contextConfig.[MasonContext.Properties.SolutionDirKey] <- locationDir.FullName
                    shouldTraverse <- false
                else
                    locationDir <- locationDir.Parent
                    if (locationDir = null) then shouldTraverse <- false
        | None -> ()

        configs <- Array.append configs [|
                contextConfig;
                EnvironmentProperties(EnvironmentVariableTarget.Process); 
                EnvironmentProperties(EnvironmentVariableTarget.User); 
            |]
        ExpandableProperties(PropertiesChain(configs))




