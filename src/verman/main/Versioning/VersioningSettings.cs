using System.Collections.Generic;
using System.Text;

using Mason.Sdk;


namespace Mason.Versioning
{
    public sealed class VersioningSettings : AbstractMasonSettings, IMasonProperties
    {
        public static class Properties
        {
            public const string VersionPropertyToUpdateKey = "mason.verman.version-property-to-update";
            public const string ConfigFileEncodingKey = "mason.verman.config-file-encoding";
            public const string VersionKey = "version";
            public const string VersionMajorKey = "version.major";
            public const string VersionMinorKey = "version.minor";
            public const string VersionBuildKey = "version.build";
            public const string VersionRevisionKey = "version.revision";
            public const string VersionIncrementStrategyKey = "mason.verman.version-increment-strategy";
        }

        private readonly IMasonProperties _properties;
        
        public VersioningSettings(IMasonProperties properties) : base(properties)
        {
            _properties = properties;
            VersionPropertyToUpdate = GetRequiredProperty(Properties.VersionPropertyToUpdateKey);
            ConfigFileEncoding = Encoding.GetEncoding(GetRequiredProperty(Properties.ConfigFileEncodingKey));
            VersionMajor = int.Parse(GetRequiredProperty(Properties.VersionMajorKey));
            VersionMinor = int.Parse(GetRequiredProperty(Properties.VersionMinorKey));
            VersionBuild = int.Parse(GetRequiredProperty(Properties.VersionBuildKey));
            VersionRevision = int.Parse(GetRequiredProperty(Properties.VersionRevisionKey));
            VersionIncrementStrategy = GetRequiredProperty(Properties.VersionIncrementStrategyKey);
        }

        public string VersionPropertyToUpdate { get; }
        public Encoding ConfigFileEncoding { get; }
        public int VersionMajor { get; }
        public int VersionMinor { get; }
        public int VersionBuild { get; }
        public int VersionRevision { get; }
        public string VersionIncrementStrategy { get; }

        public string this[string property] => _properties[property];
        IEnumerable<string> IMasonProperties.Keys => _properties.Keys;
    }
}
