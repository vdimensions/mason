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
                [VersioningSettings.Properties.ConfigFileEncodingKey] = "UTF-8",
                [VersioningSettings.Properties.VersionMajorKey] = "0",
                [VersioningSettings.Properties.VersionMinorKey] = "0",
                [VersioningSettings.Properties.VersionBuildKey] = "0",
                [VersioningSettings.Properties.VersionRevisionKey] = "1",
                [VersioningSettings.Properties.VersionKey] = string.Format(
                        "${{{0}}}.${{{1}}}.${{{2}}}.${{{3}}}",
                        VersioningSettings.Properties.VersionMajorKey,
                        VersioningSettings.Properties.VersionMinorKey,
                        VersioningSettings.Properties.VersionBuildKey,
                        VersioningSettings.Properties.VersionRevisionKey),
                [VersioningSettings.Properties.VersionIncrementStrategyKey] = MSBuildVersionIncrementer.Name,
            };
            return new VersioningSettings(new PropertiesChain(properties, defaults));
        }

        private readonly IDictionary<string, VersionIncrementer> _versionIncrementers = new Dictionary<string, VersionIncrementer>(StringComparer.OrdinalIgnoreCase)
        {
            {SimpleVersionIncrementer.Name, new SimpleVersionIncrementer()},
            {MSBuildVersionIncrementer.Name, new MSBuildVersionIncrementer()}
        };

        public void UpdateConfig(VersioningSettings settings, IDictionary<string, string> newData)
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
            if (_versionIncrementers.TryGetValue(settings.VersionIncrementStrategy, out var incrementer))
            {
                UpdateConfig(settings, incrementer.Update(settings));
            }
            else
            {
                throw new NotSupportedException($"Unknown version increment strategy '{settings.VersionIncrementStrategy}'. ");
            }
        }

        public override string Name => "verman";
    }
}



