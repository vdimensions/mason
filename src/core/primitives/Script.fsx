#r "bin/Debug/net40/Mason.Primitives.dll"
#r "System.Core"
#r "System"
open System

open Mason

let properties = MasonConfiguration.Get(@"I:\Projects\VDimensions\projects\mason-github\src\core\primitives", "Mason.Primitives.fsproj")
for k in properties.Keys do Console.WriteLine("{0} = {1}", k, properties.[k])
