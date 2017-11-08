namespace Mason

open System.Collections.Generic
open System.Linq
open System.Text.RegularExpressions

[<AutoOpen>]
module Expander =
    val expand: IMasonProperties option * string option -> string option