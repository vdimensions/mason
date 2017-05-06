using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Mason.Config;
using Mason.Distribution;

namespace Mason.Packaging
{
    public static class PackagingManager
    {
        private const string MessageCannotRunFormat = "Cannot run distribution task. Property '{0}' is not defined in build.properties";
        private const string PathSeparator = ";";
        private const string CommandSeparator = "|";
        private const string PackageIncludeFile = "mason.nuspec-includes.txt";

        public static void Distribute(string location, string projectName, Encoding encoding)
        {
            Directory.SetCurrentDirectory(location);
            var config = BuildConfiguration.Get(location, projectName, encoding);
            var projectPackageConfig = Path.Combine(location, string.Format("{0}.{1}", projectName, PackageIncludeFile));
           
            ProcessPackageIncludes(location, projectName, projectPackageConfig, config, encoding);

            var settings = new PackagingSettings(config);
            var outputLocation = settings.OutputLocation;
            if (outputLocation == null)
            {
                Console.WriteLine(MessageCannotRunFormat, PackagingSettings.Properties.OutputLocation);
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
                    process.WaitForExit( );
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

        private static void ProcessPackageIncludes(string location, string projectName, string projectPackageConfig, IBuildConfig config, Encoding encoding)
        {
            PackageContents includes = null;
            if (File.Exists(string.Format("{0}.{1}", projectName, PackageIncludeFile)))
            {
                includes = new PackageContents(projectPackageConfig, config);
            }
            else if (File.Exists(Path.Combine(location, PackageIncludeFile)))
            {
                includes = new PackageContents(Path.Combine(location, PackageIncludeFile), config);
            }
            if (includes != null)
            {
                var expander = new Expander();
                var nupkg = Path.Combine(location, expander.Expand(config, "${id}.nuspec"));
                if (!File.Exists(nupkg))
                {
                    return;
                }
                using (var reader = new StreamReader(nupkg, encoding))
                {
                    var doc = XDocument.Load(reader);
                    var filesEelement = doc.Root.Element(XName.Get("files"));
                    foreach (var packageInclude in includes)
                    {
                        var includeElement = new XElement(
                            "file", 
                            new XAttribute("src", packageInclude.Source),
                            new XAttribute("target", packageInclude.Destination));
                        filesEelement.Add(includeElement);
                    }
                    using (var writer = new StreamWriter(nupkg, false, encoding))
                    {
                        doc.Save(writer);
                    }
                }
            }
        }

        private static string GetCommonPackagePart(FileInfo file)
        {
            var nameWithExtension = file.FullName.Substring(file.DirectoryName.Length);
            var parts = nameWithExtension.Split('.');
            return string.Join(".", parts.Reverse().Skip(3).Reverse().ToArray());
        }
    }
}

