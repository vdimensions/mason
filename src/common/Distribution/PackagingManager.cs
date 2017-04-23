using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Mason.Config;


namespace Mason.Distribution
{
    public static class PackagingManager
    {
        

        private const string MessageCannotRunFormat = "Cannot run distribution task. Property '{0}' is not defined in build.properties";
        private const string PathSeparator = ";";
        private const string CommandSeparator = "|";

        public static void Distribute(IBuildConfig config, string location)
        {
            Directory.SetCurrentDirectory(location);

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

        private static string GetCommonPackagePart(FileInfo file)
        {
            var nameWithExtension = file.FullName.Substring(file.DirectoryName.Length);
            var parts = nameWithExtension.Split('.');
            return string.Join(".", parts.Reverse().Skip(3).Reverse().ToArray());
        }
    }
}

