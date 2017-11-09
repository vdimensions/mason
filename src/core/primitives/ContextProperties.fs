namespace Mason

open System
open System.Collections.Generic


type ContextProperties(contextName: string) as self =
    let _rawData: Dictionary<string, string> = Dictionary<string, string>(StringComparer.Ordinal)
    do
        if (obj.ReferenceEquals(null, contextName)) then raise (ArgumentNullException "contextName")
    new() = ContextProperties("")
    member __.Keys with get() = _rawData.Keys :> seq<string>
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k ->
                            let actualKey = contextName + k;
                            match _rawData.TryGetValue actualKey with 
                            | (true, value) -> value 
                            | (false, notFound) -> notFound
                        | None -> raise (ArgumentNullException("key"))
                    and set key value = 
                        match null2opt key with
                        | Some k -> 
                            let actualKey = contextName + k;
                            _rawData.[actualKey] <- value
                            ()
                        | None -> raise (ArgumentNullException "key")
    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]