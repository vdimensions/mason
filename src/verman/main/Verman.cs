using System;
using System.IO;
using System.Text;

using Mason.Config;
using Mason.Sdk;
using Mason.Versioning;


namespace Mason
{
    internal class Verman : AbstractMasonModule<VersioningSettings>
    {
        private const string MessageCannotRunFormat = "Cannot run version manager. Property '{0}' is not defined in {1}";

        public static void Main(string[] args)
        {
            var location = args[0];
            var projectName = args.Length > 1 ? args[1] : null;

            VersionManager.IncreaseVersion(location, projectName, Encoding.UTF8);
        }

        public override VersioningSettings CreateConfiguration(IMasonProperties properties)
        {
            var defaults = new ContextProperties
            {
                [VersioningSettings.Properties.VersionPropertyToUpdateKey] = "version.revision"
            };
            return new VersioningSettings(new PropertiesChain(properties, defaults));
        }

        public override void Run(VersioningSettings settings, params string[] args)
        {
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
            projectConfig.UpdateValue(versionPropertyToUpdate, (++version).ToString()).UpdateConfig();
        }

        public override string Name => "verman";
    }
}

