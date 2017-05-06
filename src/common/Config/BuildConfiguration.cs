﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Mason.Config
{
    public static class BuildConfiguration
    {
        public const string DefaultBuildConfigFileName = "mason.properties";

        public static IBuildConfig Get(string location, string projectName, Encoding encoding, string buildConfigFileName)
        {
            var directories = FileSystem.EnumerateDirectories(
                location,
                dir => File.Exists(Path.Combine(dir, buildConfigFileName)),
                dir => Directory.GetFiles(dir, Constants.SolutionFilePattern).Length > 0);

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
            }
            foreach (var dir in directories)
            {
                configs.Add(new BuildConfig(Path.Combine(dir, buildConfigFileName), encoding));
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

