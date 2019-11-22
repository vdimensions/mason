namespace Mason.Packaging
{
    public class PackageCreateSettings : PackagingSettings
    {
        new public static class Properties
        {
            public const string OutputLocation       = PackagingSettings.Properties.OutputLocation;
            public const string OutputFilePatterns   = "mason.packager.output.file-patterns";
            public const string Commands             = "mason.packager.commands";
        }

        public PackageCreateSettings(IMasonProperties properties) : base(properties)
        {
            OutputFilePatterns = properties[Properties.OutputFilePatterns];
            Commands = properties[Properties.Commands];
        }

        public string OutputFilePatterns { get; }
        public string Commands { get; }
    }
}