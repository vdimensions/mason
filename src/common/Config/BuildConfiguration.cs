using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Mason.Config
{
    public static class BuildConfiguration
    {
        public const string DefaultBuildConfigFileName = "mason.properties";

        public static IBuildConfig Get(string location, string projectName, Encoding encoding, string buildConfigFileName)
        {
            FileSystem.LocationPredicate endDirFunc = dir => Directory.GetFiles(dir, Constants.SolutionFilePattern).Length > 0;
            var configs = new List<IBuildConfig>( );
            if (!string.IsNullOrEmpty(projectName))
            {   //
                // Adding project specific configuration with higher priority if present
                //
                var projectConfig = Path.Combine(location, $"{projectName}.{buildConfigFileName}");
                if (File.Exists(projectConfig))
                {
                    configs.Add(new BuildConfig(projectConfig, encoding));
                }
                var defaultConfig = Path.Combine(location, buildConfigFileName);
                if (File.Exists(defaultConfig))
                {
                    configs.Add(new BuildConfig(defaultConfig, encoding));
                }
                var depthLevelRaw = configs.Select(x => x["mason.configuration.probing-depth-limit"]).FirstOrDefault(x => x!= null);
                var parentDir = location;
                if (depthLevelRaw != null && int.TryParse(depthLevelRaw, out int depthLevel) && depthLevel > 0)
                {
                    while (depthLevel > 0)
                    {
                        depthLevel--;
                        var nextParent = new DirectoryInfo(parentDir)?.Parent?.FullName;
                        if (nextParent == null)
                        {
                            break;
                        }
                        parentDir = nextParent;
                    }
                    endDirFunc = dir => new DirectoryInfo(dir).FullName.Equals(new DirectoryInfo(parentDir).FullName);
                }
            }

            var directories = FileSystem.EnumerateDirectories(
                location,
                dir => File.Exists(Path.Combine(dir, buildConfigFileName)),
                endDirFunc)
            .Skip(1);

            foreach (var dir in directories)
            {
                var path = Path.Combine(dir, buildConfigFileName);
                if (File.Exists(path))
                {
                    configs.Add(new BuildConfig(Path.Combine(dir, buildConfigFileName), encoding));
                }
            }
            configs.Add(new EnvironmentConfig(EnvironmentVariableTarget.Process));
            configs.Add(new EnvironmentConfig(EnvironmentVariableTarget.User));
            configs.Add(new EnvironmentConfig(EnvironmentVariableTarget.Machine));

            return new ExpandingConfig(new BuildConfigChain(configs));
        }
        public static IBuildConfig Get(string location, string projectName, Encoding encoding)
        {
            return Get(location, projectName, encoding, DefaultBuildConfigFileName);
        }
    }
}

