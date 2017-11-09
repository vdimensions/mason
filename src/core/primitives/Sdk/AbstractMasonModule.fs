namespace Mason.Sdk

open Mason
open System

[<AbstractClass>]
type AbstractMasonModule<'TSettings>() as self = 
    abstract member CreateConfiguration: properties: IMasonProperties -> 'TSettings

    abstract member Run: settings: 'TSettings * [<ParamArray>] args: string array -> unit

    abstract member Run: properties: IMasonProperties * [<ParamArray>] args: string array -> unit
    default this.Run(properties, args) = self.Run(this.CreateConfiguration(properties), args)

    abstract member Name: string with get

    interface IMasonModule with
        member __.Run(properties, args) = self.Run(properties, args)
        member __.Name with get() = self.Name
