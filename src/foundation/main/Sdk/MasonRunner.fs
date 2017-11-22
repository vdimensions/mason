namespace Mason.Sdk

open System
open System.IO
open System.Reflection
open System.Text.RegularExpressions
open Mason
open Mason.Sdk.Options

module MasonRunner =
    [<Literal>]
    let AssemblySearchPattern = "*.dll"
    /// Regex pattern for assemblies that must be skipped while looking for plugins, to speed up the execution
    [<Literal>] 
    let ModuleAssemblyExclusionPattern: string = "(FSharp.*\.dll)|(System.*\.dll)|(Kajabity.*\.dll)|(Mono.*\.dll)|(Mason\.exe)";
    [<Literal>]
    let Options: RegexOptions = RegexOptions.Compiled|||RegexOptions.CultureInvariant|||RegexOptions.IgnoreCase|||RegexOptions.Multiline;

    let _expression = new Regex(ModuleAssemblyExclusionPattern, Options);

    let filterAssemblies files: seq<string> =
        files |> Seq.filter (fun f -> not (_expression.IsMatch(Path.GetFileName(f)) ) )

    let loadAssembly file =
        try 
            Some(Assembly.LoadFrom(file)) 
        with 
        | _ -> None

    let getAssemblies() =
        let probingDirectories: string list = [
            AppDomain.CurrentDomain.BaseDirectory;
            ]
        probingDirectories 
        |> Seq.map (fun d -> Directory.EnumerateFiles(d, AssemblySearchPattern) |> filterAssemblies)
        |> Seq.collect (fun files -> files |> Seq.map loadAssembly)
        |> Seq.choose id |> Seq.toList

    let isModuleType (t: Type) = 
        t.IsClass && not t.IsAbstract && typedefof<IMasonModule>.IsAssignableFrom(t)

    let getPluginTypes (asm: Assembly) = 
        asm.GetTypes() 
        |> Seq.filter isModuleType

    let instantiateType (t: Type) = 
        Activator.CreateInstance(t) :?> IMasonModule
    
    let getModules() = 
        getAssemblies() 
        |> Seq.collect getPluginTypes 
        |> Seq.map instantiateType

    let Run(optionParser: IOptionParser, args) = 
        let modules = getModules();
        printfn "Modules count %i" (Array.ofSeq modules).Length
        match List.ofArray args with
        | [] | [_] -> 
            printfn "Insufficient arguments"
            ()
        | candidateModule::remainder ->
            let cmp = StringComparer.OrdinalIgnoreCase;
            match modules |> Seq.tryFind (fun m -> cmp.Equals(m.Name, candidateModule)) with 
            | Some m -> 
                printfn "Executing module %s" m.Name
                // TODO: let the module itself determine if a project name was specified
                let projectName = remainder.Head
                let options = optionParser.Parse(Array.ofList remainder.Tail)
                let properties = MasonConfiguration.Get(Environment.CurrentDirectory, projectName)
                m.Run(properties, options)
            | None -> ()

