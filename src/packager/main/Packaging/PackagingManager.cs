using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Mason.Packaging
{
    public static class PackagingManager
    {
        private const string MessageCannotRunFormat = "Cannot run distribution task. Property '{0}' is not defined in mason.properties";
        private const string PathSeparator = ";";
        private const string CommandSeparator = "|";
        private const string PackageIncludeFile = "mason.nuspec-includes.txt";

        public static void Distribute(string location, string projectName)
        {
            Directory.SetCurrentDirectory(location);
            var config = MasonConfiguration.Get(location, projectName);
            var projectPackageConfig = Path.Combine(location, $"{projectName}.{PackageIncludeFile}");
            var settings = new PackageCreateSettings(config);

            ProcessPackageIncludes(
                config,
                settings,
                location, 
                projectName, 
                projectPackageConfig);
            
            var outputLocation = settings.OutputLocation;
            if (outputLocation == null)
            {
                Console.WriteLine(MessageCannotRunFormat, PackageCreateSettings.Properties.OutputLocation);
                return;
            }
            if (!Directory.Exists(outputLocation))
            {
                Directory.CreateDirectory(outputLocation);
            }

            var cfgCommands = settings.Commands;
            if (cfgCommands != null)
            {
                var commands = cfgCommands.Split(new[] { CommandSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var command in commands)
                {
                    Console.WriteLine("Excuting command `{0}`", command);

                    var indexOfSpace = command.IndexOf(' ');
                    var cmdName = command.Substring(0, indexOfSpace);
                    var cmdArgs = command.Substring(indexOfSpace + 1);

                    var startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = cmdName,
                        Arguments = cmdArgs
                    };

                    var process = new Process { StartInfo = startInfo };
                    process.Start();
                    process.WaitForExit();
                }
            }

            var outputFilePatterns = settings.OutputFilePatterns;
            if (outputFilePatterns != null)
            {
                foreach (var pattern in outputFilePatterns.Split(new[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries))
                foreach (var file in Directory.GetFiles(location, pattern))
                {
                    var fi = new FileInfo(file);
                    fi.CopyTo(Path.Combine(outputLocation, fi.Name), true);
                }
            }

            if (settings.OutputAutoRemove)
            {
                var groups = Directory.GetFiles(outputLocation)
                    .Select(x => new FileInfo(x))
                    .Where(x => x.Extension.EndsWith("nupkg", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(GetCommonPackagePart, StringComparer.OrdinalIgnoreCase);
                foreach (var gr in groups)
                foreach (var fi in gr.OrderByDescending(y => y.CreationTime).Skip(1))
                {
                    fi.Delete( );
                }
            }
        }

        private static void ProcessPackageIncludes(
            IMasonProperties config,
            PackageCreateSettings settings,
            string location, 
            string projectName, 
            string projectPackageConfig)
        {
            PackageContents includes = null;
            if (File.Exists($"{projectName}.{PackageIncludeFile}"))
            {
                includes = new PackageContents(projectPackageConfig);
            }
            else if (File.Exists(Path.Combine(location, PackageIncludeFile)))
            {
                includes = new PackageContents(Path.Combine(location, PackageIncludeFile));
            }
            if (includes != null)
            {
                var nupkg = Path.Combine(location, Expander.Expand(config, "${id}.nuspec"));
                if (!File.Exists(nupkg))
                {
                    return;
                }
                XDocument doc;
                using (var reader = new StreamReader(nupkg, Encoding.UTF8))
                {
                    doc = XDocument.Load(reader);
                }
                if (doc?.Root == null)
                {
                    return;
                }
                var filesEelement = doc.Root.Element(XName.Get("files"));
                if (filesEelement == null)
                {
                    doc.Root.Add(new XElement("files"));
                    filesEelement = doc.Root.Element(XName.Get("files"));
                }
                foreach (var packageInclude in includes)
                {
                    if (settings.ExcludeMissingFiles && !File.Exists(Path.Combine(location, packageInclude.Source)))
                    {
                        continue;                        
                    }
                    var includeElement = new XElement(
                        "file",
                        new XAttribute("src", Expander.Expand(config, packageInclude.Source)),
                        new XAttribute("target", Expander.Expand(config, packageInclude.Destination)));
                    filesEelement.Add(includeElement);
                }

                using (var writer = new StreamWriter(nupkg, false, Encoding.UTF8))
                {
                    doc.Save(writer);
                }
            }
        }

        private static string GetCommonPackagePart(FileInfo file)
        {
            var nameWithExtension = file.FullName.Substring((file.DirectoryName?.Length).GetValueOrDefault(0));
            var parts = nameWithExtension.Split('.');
            return string.Join(".", parts.Reverse().Skip(3).Reverse().ToArray());
        }
    }
}

