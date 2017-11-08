namespace Mason


type ExpandableProperties(properties: IMasonProperties) as self =
    member __.Keys with get() = properties.Keys
    member __.Item with get(key) = expand(null2opt properties, properties.[key])

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]




