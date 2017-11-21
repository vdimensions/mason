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

    let getAssemblies() =
        let probingDirectories: string list = [
            AppDomain.CurrentDomain.BaseDirectory;
            ]
        probingDirectories 
        |> Seq.map (fun d -> Directory.EnumerateFiles(d, AssemblySearchPattern) |> filterAssemblies)
        |> Seq.collect (fun files -> files |> Seq.map (fun f -> try Some(Assembly.LoadFrom(f)) with | _ -> None))
        |> Seq.choose id |> Seq.toList

    let getPluginTypes (asm: Assembly) = 
        asm.GetTypes() 
        |> Seq.filter (fun t -> t.IsClass && not t.IsAbstract && typedefof<IMasonModule>.IsAssignableFrom(t))

    let instantiateType (t: Type) = 
        Activator.CreateInstance(t) :?> IMasonModule
    
    let getModules() = 
        getAssemblies() 
        |> Seq.collect getPluginTypes 
        |> Seq.map instantiateType

    let Run(optionParser: IOptionParser, args) = 
        let modules = getModules();
        match List.ofArray args with
        | [] | [_] -> 
            System.Console.WriteLine("Insufficient arguments")
            ()
        | candidateModule::remainder ->
            let cmp = StringComparer.OrdinalIgnoreCase;
            match modules |> Seq.tryFind (fun m -> cmp.Equals(m.Name, candidateModule)) with 
            | Some m -> 
                System.Console.WriteLine("Executing module {0}", m.Name)
                // TODO: let the module itself determine if a project name was specified
                let projectName = remainder.Head
                let options = optionParser.Parse(Array.ofList remainder.Tail)
                let properties = MasonConfiguration.Get(Environment.CurrentDirectory, projectName)
                m.Run(properties, options)
            | None -> ()

