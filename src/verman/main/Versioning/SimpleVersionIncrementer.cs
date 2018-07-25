using System;
using System.Collections.Generic;
using System.Globalization;


namespace Mason.Versioning
{
    public sealed class SimpleVersionIncrementer : VersionIncrementer
    {
        public const string Name = "simple";

        public override IDictionary<string, string> Update(VersioningSettings versionSettings)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                {VersioningSettings.Properties.VersionRevisionKey, (versionSettings.VersionRevision+1).ToString(CultureInfo.InvariantCulture)},
            };
        }
    }
}