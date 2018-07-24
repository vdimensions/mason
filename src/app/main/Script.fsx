#r "bin/Debug/Kajabity.Tools.Java.dll"
#r "bin/Debug/Mason.Foundation.dll"
#r "bin/Debug/Mason.Packager.dll"
#r "bin/Debug/Mason.VersionManager.dll"
#r "bin/Debug/mason.exe"
#r "System.Core"
#r "System"

System.Reflection.Assembly.LoadFile("D:\\Ivaylo\\Projects\\mason\\src\\app\\main\\bin\\Debug\\Mason.Packager.dll");
System.Reflection.Assembly.LoadFile("D:\\Ivaylo\\Projects\\mason\\src\\app\\main\\bin\\Debug\\Mason.VersionManager.dll");


Program.main [|"verman"; "D:\\Ivaylo\\Projects\\mason\\src\\packager\\targets\\Mason.Targets.Packager.fsproj"|]
Program.main [|"pack"; "D:\\Ivaylo\\Projects\\mason\\src\\packager\\targets\\Mason.Targets.Packager.fsproj"; "create"|]