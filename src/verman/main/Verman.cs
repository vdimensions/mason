using System;
using System.Collections.Generic;
using System.IO;

using Mason.Sdk;
using Mason.Versioning;


namespace Mason
{
    internal class VermanModule : AbstractMasonModule<VersioningSettings>
    {
        public override VersioningSettings CreateConfiguration(IMasonProperties properties)
        {           
            var defaults = new DefaultProperties
            {
                [VersioningSettings.Properties.VersionPropertyToUpdateKey] = "version.revision",
                [VersioningSettings.Properties.ConfigFileEncodingKey] = "UTF-8"
            };
            return new VersioningSettings(new PropertiesChain(properties, defaults));
        }

        public void UpdateConfig(VersioningSettings settings, IDictionary<string, object> newData)
        {
            var lines = File.ReadAllLines(settings.ConfigFile.FullName, settings.ConfigFileEncoding);
            var updates = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                foreach (var updatedKey in newData.Keys)
                {
                    if (line.TrimStart().StartsWith(updatedKey))
                    {
                        lines[i] = $"{updatedKey} = {newData[updatedKey]}";
                        updates++;
                    }
                }
            }
            if (updates > 0)
            {
                File.WriteAllLines(settings.ConfigFile.FullName, lines, settings.ConfigFileEncoding);
            }
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
            UpdateConfig(settings, new Dictionary<string, object>{{versionPropertyName, ++version}});
        }

        public override string Name => "verman";
    }
}

