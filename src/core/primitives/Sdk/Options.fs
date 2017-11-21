namespace Mason.Sdk

open System.Collections.Generic
open Mason

module Options =
    open System
    
    [<Interface>]
    type IOptionMap = abstract member TryResolve<'a> : name:string -> bool*'a

    [<Class>]
    type OptionDictionary(data: IDictionary<string, obj>) as self =
        do match null2opt data with | None -> nullArg "data"
        new () = OptionDictionary(Dictionary<string, obj>(StringComparer.OrdinalIgnoreCase))
        member __.TryResolve<'a>(name:string) =
            match data.TryGetValue name with
            | (true, v) -> 
                match v with
                | :?'a as  vv  -> (true, vv)
                | _ -> (false, Unchecked.defaultof<'a>)
            | (false, _) -> (false, Unchecked.defaultof<'a>)

        interface IOptionMap with member __.TryResolve<'a>(name) = self.TryResolve<'a> name

    [<Interface>]
    type IOptionParser = 
        abstract member Parse : args:string array -> IOptionMap


