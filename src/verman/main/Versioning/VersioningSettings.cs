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
        }

        private readonly IMasonProperties _properties;
        
        public VersioningSettings(IMasonProperties properties) : base(properties)
        {
            _properties = properties;
            VersionPropertyToUpdate = GetRequiredProperty(Properties.VersionPropertyToUpdateKey);
            ConfigFileEncoding = Encoding.GetEncoding(GetRequiredProperty(Properties.ConfigFileEncodingKey));
        }

        public string VersionPropertyToUpdate { get; }
        public Encoding ConfigFileEncoding { get; }

        public string this[string property] => _properties[property];
        IEnumerable<string> IMasonProperties.Keys => _properties.Keys;
    }
}
