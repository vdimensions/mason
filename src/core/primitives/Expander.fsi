namespace Mason

open System.Collections.Generic
open System.Linq
open System.Text.RegularExpressions

[<AutoOpen>]
module Expander =
    /// <summary>
    /// Replaces all occurences of token placeholders (items between <c>${</c> and <c>}</c>) 
    /// in a given <paramref name="text" /> with the values obtained from the <paramref name="properties" />
    /// configuration, that correspond to the respective key.
    /// </summary>
    val Expand: properties: IMasonProperties * text: string -> string
