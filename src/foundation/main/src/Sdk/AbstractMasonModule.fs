namespace Mason.Sdk

open Mason
open Mason.Sdk.Options

[<AbstractClass>]
type AbstractMasonModule<'TSettings>() as self = 
    abstract member CreateConfiguration: properties: IMasonProperties -> 'TSettings

    //abstract member ParseOptions : parser: IOptionParser * args: string array -> unit

    abstract member Run: settings: 'TSettings * options: IOptionMap -> unit
    abstract member Run: properties: IMasonProperties * options: IOptionMap -> unit
    default this.Run(properties, args) = self.Run(this.CreateConfiguration(properties), args)

    abstract member Name: string with get

    interface IMasonModule with
        //member __.ParseOptions (parser: IOptionParser, args: string array) = self.ParseOptions(parser, args)
        member __.Run(properties, args) = self.Run(properties, args)
        member __.Name with get() = self.Name
