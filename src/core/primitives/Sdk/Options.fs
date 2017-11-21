namespace Mason.Sdk

open System.Collections.Generic
open Mason

module Options =
    open System
    
    [<Interface>]
    type IOptionMap = 
        abstract member Resolve<'a> : name:string -> 'a
        abstract member ResolveFlag : name:string -> bool

    [<Class>]
    type OptionDictionary(data: IDictionary<string, obj>) as self =
        do match null2opt data with | None -> nullArg "data" | Some _ -> ()
        new () = OptionDictionary(Dictionary<string, obj>(StringComparer.OrdinalIgnoreCase))
        member __.Resolve<'a>(name:string) =
            match data.TryGetValue name with
            | (true, v) -> 
                match v with
                | :?'a as  vv -> vv
                | _ -> Unchecked.defaultof<'a>
            | (false, _) -> Unchecked.defaultof<'a>
        member __.ResolveFlag(name:string) =
            match data.TryGetValue name with
            | (true, v) -> 
                match null2opt v with
                | Some vv -> true
                | None -> false
            | (false, _) -> false

        interface IOptionMap with 
            member __.Resolve<'a>(name) = self.Resolve<'a> name
            member __.ResolveFlag(name) = self.ResolveFlag name

    [<Interface>]
    type IOptionParser = 
        abstract member Parse : args:string array -> IOptionMap


