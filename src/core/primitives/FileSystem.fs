namespace Mason

open System
open System.IO

[<Obsolete>]
module FileSystem =
    type LocationPredicate = delegate of location: DirectoryInfo -> bool

    let always(s: DirectoryInfo) = true
    let never(s: DirectoryInfo) = false

    let rec recEnumerateDirectories(location: DirectoryInfo, filter: LocationPredicate, terminator: LocationPredicate) =
        match null2opt location with
        | Some loc ->
            if (terminator.Invoke(loc)) then []
            else if (filter.Invoke(loc)) then 
                if (loc = loc.Root) then [loc]
                else [loc] @ recEnumerateDirectories(loc.Parent, filter, terminator)
            else
                if (loc = location.Root) then [loc]
                else [loc] @ recEnumerateDirectories(loc.Parent, filter, terminator)
        | None -> []

    let EnumerateDirectories(location: string, filter: LocationPredicate, terminateCondition: LocationPredicate) = 
        let f = match null2opt filter with | Some fn -> fn | None -> LocationPredicate always
        let t = match null2opt terminateCondition with | Some fn -> fn | None -> LocationPredicate never
        match null2opt location with 
        | Some loc -> recEnumerateDirectories(new DirectoryInfo(loc), f, t) :> seq<DirectoryInfo>
        | None -> raise (ArgumentNullException("location"))

    let SearchDirectories(option: SearchOption, location: string, pattern: string) = 
        match null2opt location with 
        | Some loc ->
            match null2opt pattern with 
            | Some p -> DirectoryInfo(loc).GetFiles(p, option) :> seq<FileInfo>
            | None -> raise (ArgumentNullException("pattern"))
        | None -> raise (ArgumentNullException("location"))
        
            

