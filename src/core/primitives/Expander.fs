namespace Mason

open System.Linq
open System.Text.RegularExpressions

[<AutoOpen>]
module Expander =
    [<Literal>] 
    let MatchGroupName: string = "exp"
    [<Literal>] 
    let RegExPattern: string = @"(?<" + MatchGroupName + @">(?:\${)(?:<" + MatchGroupName + @">|(?:[^}])+)+(?:}))";
    [<Literal>]
    let Options: RegexOptions = RegexOptions.Compiled|||RegexOptions.CultureInvariant|||RegexOptions.IgnoreCase|||RegexOptions.Multiline|||RegexOptions.ExplicitCapture;

    let _expression = new Regex(RegExPattern, Options);

    let expand(properties: IMasonProperties option, input: string option): string option =
        let rec recExpand(props : IMasonProperties, input: string): string =
            let mutable result = input
            for token in _expression.Matches(input).Cast<Match>().SelectMany(fun m -> _expression.GetGroupNames().Select(fun g -> m.Groups.[g].Value)).ToList() do
                let nakedToken = token.[2..token.Length - 3]
                result <- match props.[nakedToken] with | Some value -> recExpand(props, result.Replace(token, value)) | None -> result
            result

        match properties with
        | Some props ->
            match input with
            | Some inp -> Some (recExpand (props, inp))
            | None -> None
        | None -> None
