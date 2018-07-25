using System;
using System.Collections.Generic;
using System.Globalization;


namespace Mason.Versioning
{
    public sealed class MSBuildVersionIncrementer : VersionIncrementer
    {
        public const string Name = "MSBuild";

        //<Build>$([System.DateTime]::op_Subtraction($([System.DateTime]::get_Now().get_Date()),$([System.DateTime]::new(2000,1,1))).get_TotalDays())</Build>
        //<Revision>$([MSBuild]::Divide($([System.DateTime]::get_Now().get_TimeOfDay().get_TotalSeconds()), 2).ToString('F0'))</Revision>
        public override IDictionary<string, string> Update(VersioningSettings versionSettings)
        {
            var now = DateTime.UtcNow;
            var buildNumber = (now.Date - new DateTime(2000, 1, 1)).TotalDays.ToString("####", CultureInfo.InvariantCulture);
            var revisionNumber = (now.TimeOfDay.TotalSeconds/2d).ToString("F0");
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                    {VersioningSettings.Properties.VersionBuildKey, buildNumber},
                    {VersioningSettings.Properties.VersionRevisionKey, revisionNumber},
            };
        }
    }
}