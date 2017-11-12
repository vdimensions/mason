using System;
using System.IO;
using System.Text;

using Mason;
using Mason.Config;


namespace Mason.Versioning
{

    public static class VersionManager
    {
        public static class Properties
        {
            public const string VersionPropertyToUpdate = "mason.verman.version-property-to-update";
        }

        public static void IncreaseVersion(string location, string projectName, Encoding encoding)
        {
            var config = MasonConfiguration.Get(location, projectName, encoding);
            var projectConfigFile = new FileInfo(Path.Combine(location, projectName + "." + MasonConfiguration.DefaultBuildConfigFileName));
            var defaultConfigfile = new FileInfo(Path.Combine(location, MasonConfiguration.DefaultBuildConfigFileName));
            if (!projectConfigFile.Exists && !defaultConfigfile.Exists)
            {
                Console.WriteLine("File does not exist: {0}", Path.Combine(location, MasonConfiguration.DefaultBuildConfigFileName));
                return;
            }

            var versionHolderFile = projectConfigFile.Exists ? projectConfigFile : defaultConfigfile;

            var versionPropertyToUpdate = config[Properties.VersionPropertyToUpdate];
            if (versionPropertyToUpdate == null)
            {
                Console.WriteLine(MessageCannotRunFormat, Properties.VersionPropertyToUpdate, MasonConfiguration.DefaultBuildConfigFileName);
                return;
            }

            var projectConfig = new WriteableBuildConfig(versionHolderFile.FullName, encoding);
            var version = 0;
            if (!int.TryParse(projectConfig[versionPropertyToUpdate], out version))
            {
                Console.WriteLine(MessageCannotRunFormat, versionPropertyToUpdate, MasonConfiguration.DefaultBuildConfigFileName);
                return;
            }
            projectConfig.UpdateValue(versionPropertyToUpdate, (++version).ToString( )).UpdateConfig( );
        }
    }
}

