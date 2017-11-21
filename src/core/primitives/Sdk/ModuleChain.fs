namespace Mason.Sdk

open System
open Mason
open Mason.Sdk.Options

type ModuleChain(optionParser: IOptionParser, modules: seq<IMasonModule>) =
    do
        if (obj.ReferenceEquals(modules, null)) then raise (ArgumentNullException "submodules")
    member __.Run(args: string array) =
        match List.ofSeq args with
        | [] | [_] -> 
            System.Console.WriteLine("No args")
            ()
        | start::remainder ->
            let cmp = StringComparer.OrdinalIgnoreCase;
            //if (cmp.Equals(start, 
            let r = Array.ofList remainder.Tail
            let options = optionParser.Parse(r)
            let projectName = remainder.Head
            let properties = MasonConfiguration.Get(Environment.CurrentDirectory, projectName)
            //let candidateMultipleSubmodules = remainder.Tail.Head.Split([|'&'|], StringSplitOptions.RemoveEmptyEntries)
            //for candidateModule in candidateMultipleSubmodules |> Seq.map (fun x -> x.Trim()) do
            let candidateModule = start
            for m in modules do
                if (cmp.Equals(m.Name, candidateModule)) then
                    System.Console.WriteLine("Executing module {0}", m.Name)
                    m.Run(properties, options)
