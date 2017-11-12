using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mason.Sdk;


namespace Mason.Versioning
{
    public sealed class VersioningSettings : AbstractMasonSettings
    {
        public static class Properties
        {
            public const string VersionPropertyToUpdateKey = "mason.verman.version-property-to-update";
        }
        
        public VersioningSettings(IMasonProperties properties) : base(properties)
        {
            VersionPropertyToUpdate = GetRequiredProperty(Properties.VersionPropertyToUpdateKey);
        }

        public string VersionPropertyToUpdate { get; }
    }
}
