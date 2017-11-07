namespace Mason.Core

open System.Collections.Generic

type IMasonProperties = 
    abstract member Keys: IEnumerable<string> with get
    abstract member Item: property:string -> string with get

