namespace Mason
open System
open System.Linq
open System.Text.RegularExpressions

module Expander =

    [<Literal>] 
    let MatchGroupName: string = "exp"
    [<Literal>] 
    let RegExPattern: string = @"(?<" + MatchGroupName + @">(?:\${)(?:<" + MatchGroupName + @">|(?:[^}])+)+(?:}))";
    [<Literal>]
    let Options: RegexOptions = RegexOptions.Compiled|||RegexOptions.CultureInvariant|||RegexOptions.IgnoreCase|||RegexOptions.Multiline|||RegexOptions.ExplicitCapture;

    let _expression = new Regex(RegExPattern, Options);

    let rec recExpand(props : IMasonProperties, input: string): string =
        let mutable result = input
        for token in _expression.Matches(input).Cast<Match>().SelectMany(fun m -> _expression.GetGroupNames().Select(fun g -> m.Groups.[g].Value)).ToList() do
            let nakedToken = token.[2..token.Length - 2]
            result <- match null2opt props.[nakedToken] with | Some value -> recExpand(props, result.Replace(token, value)) | None -> result
        result

    let Expand(properties: IMasonProperties, text: string): string = 
        match null2opt properties with
        | Some props ->
            match null2opt text with
            | Some txt -> recExpand(props, txt)
            | None -> raise (ArgumentNullException("text"))
        | None -> raise (ArgumentNullException("properties"))
        

