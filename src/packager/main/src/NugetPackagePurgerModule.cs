using System;
using System.IO;
using System.Linq;

using Mason.Packaging;
using Mason.Sdk;


namespace Mason
{
    sealed class NugetPackagePurgerModule : AbstractMasonModule<PackagingSettings>
    {
        private const string MessageCannotRunFormat = "Cannot run distribution task. Property '{0}' is not defined in mason.properties";

        internal const string ModuleName = "purge";

        public override PackagingSettings CreateConfiguration(IMasonProperties properties) { return new PackagingSettings(properties); }

        public override void Run(PackagingSettings config, Options.IOptionMap options)
        {
            var outputLocation = config.OutputLocation;
            if (outputLocation == null)
            {
                Console.WriteLine(MessageCannotRunFormat, PackagingSettings.Properties.OutputLocation);
                return;
            }
            if (!Directory.Exists(outputLocation))
            {
                return;
            }
            var groups = Directory.GetFiles(outputLocation)
                .Select(x => new FileInfo(x))
                .Where(x => x.Extension.EndsWith("nupkg", StringComparison.OrdinalIgnoreCase))
                .GroupBy(GetCommonPackagePart, StringComparer.OrdinalIgnoreCase);
            foreach (var gr in groups)
            foreach (var fi in gr.OrderByDescending(y => y.Name).Skip(1))
            {
                fi.Delete();
            }
        }

        private static string GetCommonPackagePart(FileInfo file)
        {
            var nameWithExtension = file.FullName.Substring((file.DirectoryName?.Length).GetValueOrDefault(0));
            var parts = nameWithExtension.Split('.');
            return string.Join(".", parts.Reverse().Skip(3).Reverse().ToArray());
        }

        public override string Name => ModuleName;
    }
}