namespace Mason

open System
open System.Collections.Generic


type DefaultProperties() as self =
    let _rawData: Dictionary<string, string> = Dictionary<string, string>(StringComparer.Ordinal)
    member __.Keys with get() = _rawData.Keys :> seq<string>
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k ->
                            let actualKey = k;
                            match _rawData.TryGetValue actualKey with 
                            | (true, value) -> value 
                            | (false, notFound) -> notFound
                        | None -> nullArg "key" 
                    and set key value = 
                        match null2opt key with
                        | Some k -> 
                            let actualKey = k;
                            _rawData.[actualKey] <- value
                            ()
                        | None -> nullArg "key"
    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]