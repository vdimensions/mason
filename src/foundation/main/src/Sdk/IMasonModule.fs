namespace Mason.Sdk

open Mason
open Mason.Sdk.Options

[<Interface>]
type IMasonModule =
    abstract member Run: config: IMasonProperties * options: IOptionMap -> unit
    abstract member Name: string with get
