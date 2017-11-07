namespace Mason.Core

open System
open System.Collections
open System.Collections.Generic


type EnvironmentProperties(target: EnvironmentVariableTarget) as impl =
    let _rawData: Dictionary<string, string> = Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    do
        let env = Environment.GetEnvironmentVariables target
        for key in env.Keys do       
            _rawData.[string key] <- string env.[key]
    member __.Keys with get() = _rawData.Keys :> IEnumerable<string>
    member __.Item with get(property) = _rawData.[property]
    new() = EnvironmentProperties(EnvironmentVariableTarget.User)
    interface IMasonProperties with
        member __.Keys with get() = impl.Keys
        member __.Item with get(property) = impl.[property]