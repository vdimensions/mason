namespace Mason.Core

open System.Collections
open System.IO
open System.Linq
open System.Text


type JavaProperties(file: FileInfo, encoding: Encoding) as self =
    let _props = Kajabity.Tools.Java.JavaProperties()
    do
        use stream = file.OpenRead()
        _props.Load(stream, encoding)
    member __.Keys with get() = _props.Keys.Cast<string>()
    member __.Item with get(property) = _props.GetProperty(property)
    new (file: FileInfo) = JavaProperties(file, Encoding.UTF8)

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(property) = self.[property]