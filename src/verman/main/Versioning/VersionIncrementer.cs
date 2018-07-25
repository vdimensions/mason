using System.Collections.Generic;


namespace Mason.Versioning
{
    public abstract class VersionIncrementer
    {
        public abstract IDictionary<string, string> Update(VersioningSettings versionSettings);
    }
}