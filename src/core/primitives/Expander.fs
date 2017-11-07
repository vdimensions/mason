namespace Mason.Core

open System.Text.RegularExpressions

module Expander =
    [<Literal>] 
    let MatchGroupName: string = "exp"
    [<Literal>] 
    let RegExPattern: string = @"(?<" + MatchGroupName + @">(?:\${)(?:<" + MatchGroupName + @">|(?:[^}])+)+(?:}))";
    [<Literal>]
    let Options: RegexOptions = RegexOptions.Compiled|||RegexOptions.CultureInvariant|||RegexOptions.IgnoreCase|||RegexOptions.Multiline|||RegexOptions.ExplicitCapture;

    let Regex _expression = new Regex(RegExPattern, Options);

    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None