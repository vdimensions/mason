namespace Mason.Sdk

open System

open Mason

[<Interface>]
type IMasonModule =
    abstract member Run: config: IMasonProperties*[<ParamArray>] args: string array -> unit
    abstract member Name: string with get
