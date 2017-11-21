using System;
using System.Text;

using Mason.Sdk;
using Mason.Versioning;


namespace Mason
{
    internal class VermanModule : AbstractMasonModule<VersioningSettings>
    {
        private JavaProperties _writableProps;

        public override VersioningSettings CreateConfiguration(IMasonProperties properties)
        {           
            var defaults = new ContextProperties
            {
                [VersioningSettings.Properties.VersionPropertyToUpdateKey] = "version.revision",
                [VersioningSettings.Properties.ConfigFileEncodingKey] = "UTF-8"
            };
            var result = new VersioningSettings(new PropertiesChain(properties, defaults));

            var chain = properties as PropertiesChain;
            foreach (var inner in chain)
            {
                _writableProps = inner as JavaProperties;
                if (_writableProps != null)
                {
                    break;
                }
            }
            if (_writableProps == null)
            {
                _writableProps = new JavaProperties(result.ConfigFile, Encoding.GetEncoding(result.ConfigFileEncoding));
            }
            return result;
        }

        public override void Run(VersioningSettings settings, Options.IOptionMap options)
        {
            var versionPropertyName = settings.VersionPropertyToUpdate;
            var versionPropertyValue = settings.GetRequiredProperty(versionPropertyName);
            if (!int.TryParse(versionPropertyValue, out var version))
            {
                Console.WriteLine($"Invalid version number: {versionPropertyValue}");
                return;
            }
            _writableProps[versionPropertyName] = (++version).ToString();
            _writableProps.Update();
        }

        public override string Name => "verman";
    }
}

