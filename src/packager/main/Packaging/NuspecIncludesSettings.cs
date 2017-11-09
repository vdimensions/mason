using System.Collections.Generic;


namespace Mason.Packaging
{
    public class NuspecIncludesSettings : PackagingSettings, IMasonProperties
    {
        new public static class Properties
        {
            public const string OutputLocation       = PackagingSettings.Properties.OutputLocation;
            public const string ExcludeMissingFiles  = "mason.packager.exclude-missing-files";
            public const string NuspecFile           = "mason.packager.nuspec-file";
        }

        private readonly IMasonProperties _properties;

        public NuspecIncludesSettings(IMasonProperties properties) : base(properties)
        {
            _properties = properties;
            ExcludeMissingFiles = bool.TryParse(properties[Properties.ExcludeMissingFiles], out bool excludeMissing) && excludeMissing;
            NuspecFile = properties[Properties.NuspecFile];
        }

        public bool ExcludeMissingFiles { get; }
        public string NuspecFile { get; }

        IEnumerable<string> IMasonProperties.Keys => _properties.Keys;
        string IMasonProperties.this[string property] => _properties[property];
    }
}