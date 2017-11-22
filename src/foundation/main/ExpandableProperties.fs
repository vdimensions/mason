namespace Mason

open System


type ExpandableProperties(properties: IMasonProperties) as self =
    member __.Keys with get() = properties.Keys
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k ->
                            match null2opt properties.[k] with
                            | Some value -> Expand(properties, value)
                            | None -> null
                        | None -> raise (ArgumentNullException "key")

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]




