using System;
using System.Diagnostics;
using System.IO;

using Mason.Packaging;
using Mason.Sdk;


namespace Mason
{
    sealed class NugetPackageCreateModule : AbstractMasonModule<PackageCreateSettings>
    {
        private const string MessageCannotRunFormat = "Cannot run distribution task. Property '{0}' is not defined in mason.properties";
        private const string PathSeparator = ";";
        private const string CommandSeparator = "|";

        internal const string ModuleName = "create";

        public override PackageCreateSettings CreateConfiguration(IMasonProperties properties) { return new PackageCreateSettings(properties); }

        public override void Run(PackageCreateSettings config, Options.IOptionMap options)
        {
            var outputLocation = config.OutputLocation;
            if (outputLocation == null)
            {
                Console.WriteLine(MessageCannotRunFormat, PackageCreateSettings.Properties.OutputLocation);
                return;
            }
            if (!Directory.Exists(outputLocation))
            {
                Directory.CreateDirectory(outputLocation);
            }

            var cfgCommands = config.Commands;
            if (cfgCommands != null)
            {
                var commands = cfgCommands.Split(new[] { CommandSeparator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var command in commands)
                {
                    Console.WriteLine("Executing command `{0}`", command);

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

            var outputFilePatterns = config.OutputFilePatterns;
            if (outputFilePatterns != null)
            {
                foreach (var pattern in outputFilePatterns.Split(new[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries))
                foreach (var file in Directory.GetFiles(config.Location, pattern))
                {
                    var fi = new FileInfo(file);
                    fi.CopyTo(Path.Combine(outputLocation, fi.Name), true);
                }
            }
        }

        public override string Name => ModuleName;
    }
}