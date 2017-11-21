namespace Mason.Sdk

open System
open Mason.Sdk.Options

module MasonRunner =
    val Run: optionParser: IOptionParser*[<ParamArray>]args: string array -> unit