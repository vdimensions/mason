namespace Mason

open System
open System.Collections
open System.IO
open System.Linq
open System.Text

open Kajabity.Tools.Java


type JavaProperties(file: FileInfo, encoding: Encoding) as self =
    let _props = Kajabity.Tools.Java.JavaProperties()
    do
        match null2opt file with
        | Some f ->
            let e = match null2opt encoding with Some e -> e | None -> Encoding.UTF8
            // detect and remove any UTF8 BOM mark before parsing the properties to prevent a corrupted read
            let mutable fc = File.ReadAllText(f.FullName, e)
            if (fc.StartsWith("\uFEFF", StringComparison.Ordinal)) then fc <- fc.Substring(1)
            use stream = new MemoryStream(e.GetBytes(fc.ToCharArray()))
            _props.Load(stream, e)
        | None -> nullArg "file"
    member __.Keys with get() = _props.Keys.Cast<string>()
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k -> 
                            match null2opt (_props.GetProperty(k)) with
                            | Some s -> 
                                if ((s.[0] = '"') && (s.[s.Length - 1] = '"')) then s.[1..s.Length - 2]
                                else s
                            | None -> null
                        | None -> nullArg "key"
    member __.Location with get() = file.FullName
                        
    new (file: FileInfo) = JavaProperties(file, Encoding.UTF8)
    new (path: string, encoding: Encoding) = JavaProperties(new FileInfo(path), encoding)
    new (path: string) = JavaProperties(new FileInfo(path))

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]