namespace Mason

open System
open System.Collections
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text

open Kajabity.Tools.Java


type JavaProperties(file: FileInfo, encoding: Encoding) as self =
    let _props = Dictionary<string, string>(StringComparer.Ordinal)
    do
        match null2opt file with
        | Some f ->
            let props = Kajabity.Tools.Java.JavaProperties()
            let e = match null2opt encoding with Some e -> e | None -> Encoding.UTF8
            use stream = f.OpenRead()
            props.Load(stream, e)
            // transfer the properties to a dictionary to use ordinal comparison and fix BOM
            for k in props.Keys.Cast<string>() do
                // detect and remove any UTF8 BOM before parsing the properties to prevent a corrupted read
                if (k.StartsWith("\uFEFF", StringComparison.Ordinal)) then
                    let properKey = k.Substring(1)
                    if (properKey.Length > 0) then _props.[properKey] <- props.GetProperty(k)
                else 
                    _props.[k] <- props.GetProperty(k)

        | None -> nullArg "file"
    member __.Keys with get() = _props.Keys.Cast<string>()
    member __.Item with get(key) = 
                        match null2opt key with
                        | Some k -> 
                            match (_props.TryGetValue(k)) with
                            | (true, s) -> 
                                if ((s.[0] = '"') && (s.[s.Length - 1] = '"')) then s.[1..s.Length - 2]
                                else s
                            | (false, _) -> null
                        | None -> nullArg "key"
    member __.Location with get() = file.FullName
                        
    new (file: FileInfo) = JavaProperties(file, Encoding.UTF8)
    new (path: string, encoding: Encoding) = JavaProperties(new FileInfo(path), encoding)
    new (path: string) = JavaProperties(new FileInfo(path))

    interface IMasonProperties with
        member __.Keys with get() = self.Keys
        member __.Item with get(key) = self.[key]