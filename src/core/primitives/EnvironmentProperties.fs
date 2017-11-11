namespace Mason

open System
open System.Collections
open System.Collections.Generic


type EnvironmentProperties(target: EnvironmentVariableTarget) as self =
    let _rawData: Dictionary<string, string> = Dictionary<string, string>(StringComparer.Ordinal)
    do
        let env = Environment.GetEnvironmentVariables target
        for key in env.Keys do _rawData.[string key] <- string env.[key]
    member __.Keys with get() = _rawData.Keys :> seq<string>
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k ->
                            match _rawData.TryGetValue k with 
                            | (true, value) -> value 
                            | (false, notFound) -> notFound
                        | None -> nullArg "key"
    new() = EnvironmentProperties(EnvironmentVariableTarget.User)

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]