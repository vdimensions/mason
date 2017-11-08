namespace Mason

open System.Collections
open System.IO
open System.Linq
open System.Text

open Kajabity.Tools.Java


type JavaProperties(file: FileInfo, encoding: Encoding) as self =
    let _props = Kajabity.Tools.Java.JavaProperties()
    do
        use stream = file.OpenRead()
        _props.Load(stream, encoding)
    member __.Keys with get() = _props.Keys.Cast<string>()
    member __.Item with get(key) = null2opt (_props.GetProperty(key))
    new (file: FileInfo) = JavaProperties(file, Encoding.UTF8)
    new (path: string, encoding: Encoding) = JavaProperties(new FileInfo(path), encoding)
    new (path: string) = JavaProperties(new FileInfo(path))

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]