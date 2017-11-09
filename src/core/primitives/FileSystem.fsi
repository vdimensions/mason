namespace Mason

open System.IO

module FileSystem =
    type LocationPredicate = delegate of location: DirectoryInfo -> bool

    val EnumerateDirectories: location: string * LocationPredicate * LocationPredicate -> seq<DirectoryInfo>

    val SearchDirectories: option: SearchOption * location: string * pattern: string -> seq<FileInfo>