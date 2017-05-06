using Mason.Config;

namespace Mason.Distribution
{
    public class PackagingSettings
    {
        public static class Properties
        {
            public const string OutputLocation       = "mason-packager.output.location";
            public const string OutputFilePatterns   = "mason-packager.output.file-patterns";
            public const string OutputAutoremove     = "mason-packager.output.autoremove";
            public const string Commands             = "mason-packager.commands";
            public const string ExcludeMissingFiles  = "mason-packager.exclude-missing-files";
        }

        public PackagingSettings(IBuildConfig config)
        {
            OutputLocation = config[Properties.OutputLocation];
            OutputFilePatterns = config[Properties.OutputFilePatterns];
            Commands = config[Properties.Commands];
            OutputAutoRemove = bool.TryParse(config[Properties.OutputAutoremove], out bool autoremove) && autoremove;
            ExcludeMissingFiles = bool.TryParse(config[Properties.ExcludeMissingFiles], out bool excludeMissing) && excludeMissing;
        }

        public string OutputLocation { get; private set; }
        public string OutputFilePatterns { get; private set; }
        public bool OutputAutoRemove { get; private set; }
        public string Commands { get; private set; }
        public bool ExcludeMissingFiles { get; private set; }
    }
}