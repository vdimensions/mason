namespace Mason

open System
open System.Collections
open System.Collections.Generic
open System.Linq


type PropertiesChain([<ParamArray>] props : IMasonProperties array) as self =
    let _chainedProperties : seq<IMasonProperties> = (match null2opt props with | Some p -> p | None -> [||]).Cast<IMasonProperties>()
    let _keySet = HashSet<string>(StringComparer.Ordinal)
    do
        for p in props do
            for k in p.Keys do
                _keySet.Add k |> ignore

    member __.Keys with get() = _keySet :> seq<string>
    member __.Item 
        with get(key) = 
            let mutable result: string option = None
            for p in props do
                match p.[key] with
                | Some v -> result <- Some v
                | None -> ()
            result

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]

    interface IEnumerable<IMasonProperties> with
        member __.GetEnumerator() = _chainedProperties.GetEnumerator()

    interface IEnumerable with
        member __.GetEnumerator() = (_chainedProperties :> IEnumerable).GetEnumerator()

